using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using DBCS;

namespace EcardQuery
{
    public class EcardWebsiteHelper
    {
        CookieContainer cookieContainer = new CookieContainer();
        HttpClient httpClient = new HttpClient();
        Task<HttpResponseMessage> initGetResult;

        bool isLoggedIn = false;
        public bool IsLoggedIn
        {
            get
            {
                return isLoggedIn;
            }
        }

        public EcardWebsiteHelper()
        {
            httpClient.DefaultRequestHeaders.Host = "ecard.sjtu.edu.cn";
            httpClient.BaseAddress = new Uri("http://ecard.sjtu.edu.cn/");
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("UTF-8"));
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240");
            initGetResult = httpClient.GetAsync("/homeLogin.action");
        }

        /// <summary>
        /// 获取验证码图片。应当使用try-except以处理网络错误。
        /// </summary>
        /// <returns>验证码图片</returns>
        public async Task<BitmapImage> GetCheckPicAsync()
        {
            await initGetResult.AsAsyncAction();
            Random random = new Random();
            string url = "/getCheckpic.action?rand=" + (random.NextDouble() * 10000).ToString();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            BitmapImage bitmap = new BitmapImage();
            await bitmap.SetSourceAsync((await response.Content.ReadAsStreamAsync()).AsRandomAccessStream());
            return bitmap;
        }

        public enum LoginType { CardId = 1, PersonId = 2};

        /// <summary>
        /// 登录系统
        /// </summary>
        /// <param name="loginType">登录方式：1、卡号，2、学号/工号</param>
        /// <param name="name">登录名</param>
        /// <param name="passwd">密码</param>
        /// <param name="rand">验证码</param>
        /// <returns></returns>
        public async Task LoginAsync(LoginType loginType, string name, string passwd, string rand)
        {
            string postString = "imageField.x=0&imageField.y=0"
                + "&loginType=" + ((int)loginType).ToString() + "&name=" + name 
                + "&passwd=" + passwd + "&rand=" + rand + "&userType=1";//这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  
            string url = "/loginstudent.action";//地址  

            HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(postString, Encoding.ASCII, "application/x-www-form-urlencoded"));
            response.EnsureSuccessStatusCode();
            string s = await GetResponseContentStringAsync(response);
            s = s.Replace("&nbsp;", " ");
            string title = s.Substring(s.IndexOf("<title>") + 7);
            title = title.Substring(0, title.IndexOf("<"));
            string content = "";
            int index;
            while((index = s.IndexOf("<p"))>=0)
            {
                s = s.Substring(index);
                s = s.Substring(s.IndexOf(">") + 1);
                content += s.Substring(0, s.IndexOf("<"));
                content += " ";
            }

            if (content.IndexOf("验证码")>=0)
            {
                throw new ArgumentException(content, "rand");
            }
            if (content.IndexOf("密码")>=0)
            {
                throw new ArgumentException(content, "passwd");
            }
            if (content.IndexOf("频繁") >= 0)
            {
                throw new InvalidOperationException(content);
            }

            isLoggedIn = true;
        }

        List<string> historyAccountIds = new List<string>();
        public List<string> HistoryAccountIds
        {
            get
            {
                return historyAccountIds;
            }
        }

        string historyContinueUrl;
        /// <summary>
        /// 初始化交易历史查询页面，并获取账户列表。
        /// </summary>
        /// <returns></returns>
        public async Task HistoryInquiryInit()
        {
            HttpResponseMessage response; string s;
            response = await httpClient.GetAsync("/accounthisTrjn.action");
            response.EnsureSuccessStatusCode();
            s = await GetResponseContentStringAsync(response);

            historyContinueUrl = s.Substring(s.IndexOf("action=\"/accounthisTrjn.action?") + 8);
            historyContinueUrl = historyContinueUrl.Substring(0, historyContinueUrl.IndexOf("\""));

            historyAccountIds = new List<string>();
            s = s.Substring(s.IndexOf("\"account\""));
            s = s.Substring(0, s.IndexOf("</select"));
            int index;
            while ((index = s.IndexOf("value=")) >= 0)
            {
                string accountId;
                s = s.Substring(index + 7);
                accountId = s.Substring(0, s.IndexOf("\""));
                historyAccountIds.Add(accountId);
            }
        }

        /// <summary>
        /// 查询交易历史。不能查询到当天的信息。
        /// </summary>
        /// <param name="startDate">查询开始日期</param>
        /// <param name="endDate">查询结束日期</param>
        /// <returns></returns>
        public async Task<List<TranscationData>> HistoryInquire(string startDate, string endDate,string accountId)
        {
            List<TranscationData> datas = new List<TranscationData>();

            if (historyContinueUrl == null)
                throw new InvalidOperationException("查询未初始化");

            string continueUrl = await HistoryInquire_Stage1(accountId);

            string continueUrl1 = await HistoryInquire_Stage2(startDate, endDate, continueUrl);

            //await Task.Delay(1000);
            HttpResponseMessage response2 = await httpClient.PostAsync(continueUrl1, new StringContent("", Encoding.ASCII, "application/x-www-form-urlencoded"));
            response2.EnsureSuccessStatusCode();
            string s2 = await GetResponseContentStringAsync(response2);
            History_ParseDatas(datas, s2);

            string pageCountStr = s2.Substring(0, s2.IndexOf("页"));
            pageCountStr = pageCountStr.Substring(pageCountStr.LastIndexOf("共") + 1);
            int pageCount = int.Parse(pageCountStr);

            int i;
            string contunieUrl2 = "/accountconsubBrows.action";
            for (i = 2; i <= pageCount; i++)
            {
                HttpResponseMessage response3 = await httpClient.PostAsync(contunieUrl2, new StringContent("pageNum=" + i.ToString(), Encoding.ASCII, "application/x-www-form-urlencoded"));
                response3.EnsureSuccessStatusCode();
                string s3 = await GetResponseContentStringAsync(response3);
                History_ParseDatas(datas, s3);
            }

            return datas;
        }

        #region 交易历史查询中的私有函数
        private async Task<string> HistoryInquire_Stage1(string accountId)
        {
            HttpResponseMessage response = await httpClient.PostAsync(historyContinueUrl, new StringContent("account=" + accountId + "&inputObject=all&Submit=+%C8%B7+%B6%A8+", Encoding.ASCII, "application/x-www-form-urlencoded"));
            response.EnsureSuccessStatusCode();
            string s = await GetResponseContentStringAsync(response);
            string contunieUrl = s.Substring(s.IndexOf("action=\"/accounthisTrjn.action?") + 8);
            contunieUrl = contunieUrl.Substring(0, contunieUrl.IndexOf("\""));
            return contunieUrl;
        }

        private async Task<string> HistoryInquire_Stage2(string startDate, string endDate, string contunieUrl)
        {
            HttpResponseMessage response1 = await httpClient.PostAsync(contunieUrl, new StringContent("inputEndDate=" + endDate + "&inputStartDate=" + startDate, Encoding.ASCII, "application/x-www-form-urlencoded"));
            response1.EnsureSuccessStatusCode();
            string s1 = await GetResponseContentStringAsync(response1);
            string contunieUrl1 = s1.Substring(s1.IndexOf("action=") + 8);
            contunieUrl1 = "/accounthisTrjn.action" + contunieUrl1.Substring(0, contunieUrl1.IndexOf("\""));
            return contunieUrl1;
        }

        private static void History_ParseDatas(List<TranscationData> datas, string s)
        {
            int index1, index2;
            while (true)
            {
                index1 = s.IndexOf("<tr class=\"listbg\">");
                index2 = s.IndexOf("<tr class=\"listbg2\">");
                if (index1 >= 0)
                {
                    if (index2 >= 0)
                        s = s.Substring(Math.Min(index1 + 18, index2 + 19));
                    else
                        s = s.Substring(index1 + 18);
                }
                else
                {
                    if (index2 >= 0)
                        s = s.Substring(index2 + 19);
                    else
                        break;
                }
                datas.Add(History_ParseDataLine(s.Substring(0, s.IndexOf("</tr>"))));
            }
        }

        private static TranscationData History_ParseDataLine(string content)
        {
            TranscationData data = new TranscationData();

            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.Time = DateTime.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.TranscationType = content.Substring(0, content.IndexOf("</td>"));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.SubSystem = content.Substring(0, content.IndexOf("</td>"));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.Delta = decimal.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.AccountBalance = decimal.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.CardBalance = decimal.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.Id = int.Parse(content.Substring(0, content.IndexOf("</td>")));
            return data;
        }
        #endregion

        public async Task<List<TranscationData>> RealtimeInquire(string accountId)
        {
            List<TranscationData> datas = new List<TranscationData>();
            HttpResponseMessage response = await httpClient.PostAsync("/accounttodatTrjnObject.action", new StringContent("account =" + accountId + "&inputObject=all&Submit=+%C8%B7+%B6%A8+", Encoding.ASCII, "application/x-www-form-urlencoded"));
            response.EnsureSuccessStatusCode();
            string s = await GetResponseContentStringAsync(response);

            RealTime_ParseDatas(datas, s);

            string pageCountStr = s.Substring(0, s.IndexOf("页"));
            pageCountStr = pageCountStr.Substring(pageCountStr.LastIndexOf("共") + 1);
            int pageCount = int.Parse(pageCountStr);

            return datas;
        }

        private static void RealTime_ParseDatas(List<TranscationData> datas, string s)
        {
            int index1, index2;
            while (true)
            {
                index1 = s.IndexOf("<tr class=\"listbg\">");
                index2 = s.IndexOf("<tr class=\"listbg2\">");
                if (index1 >= 0)
                {
                    if (index2 >= 0)
                        s = s.Substring(Math.Min(index1 + 18, index2 + 19));
                    else
                        s = s.Substring(index1 + 18);
                }
                else
                {
                    if (index2 >= 0)
                        s = s.Substring(index2 + 19);
                    else
                        break;
                }
                datas.Add(RealTime_ParseDataLine(s.Substring(0, s.IndexOf("</tr>"))));
            }
        }

        private static TranscationData RealTime_ParseDataLine(string content)
        {
            TranscationData data = new TranscationData();

            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.Time = DateTime.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.TranscationType = content.Substring(0, content.IndexOf("</td>"));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.SubSystem = content.Substring(0, content.IndexOf("</td>"));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.Delta = decimal.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.AccountBalance = decimal.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.CardBalance = decimal.Parse(content.Substring(0, content.IndexOf("</td>")));
            content = content.Substring(content.IndexOf(">", content.IndexOf("<td") + 1) + 1);
            data.Id = int.Parse(content.Substring(0, content.IndexOf("</td>")));
            return data;
        }

        private async Task<string> GetResponseContentStringAsync(HttpResponseMessage response)
        {
            byte[] responseData = await response.Content.ReadAsByteArrayAsync();
            return (await DBCSEncoding.GetDBCSEncoding("GB2312")).GetString(responseData);
        }

        
    }

    public class TranscationData
    {
        public DateTime Time { get; set; }
        public string TranscationType { get; set; }
        public string SubSystem { get; set; }
        public decimal Delta { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal CardBalance { get; set; }
        public int Id { get; set; }
    }
}
