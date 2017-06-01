using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcardQuery.UI
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BarChartPage : ContentPage
	{
        UrlWebViewSource source;
        IList<TransactionGroup> chartData;

        public BarChartPage(IList<TransactionGroup> data)
        {
            InitializeComponent();

            this.chartData = data;

            source = new UrlWebViewSource()
            {
                Url = "Html/BarChart.html"
            };
            webView.Source = source;
            webView.Navigated += WebView_Navigated;
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var dateAscending = chartData.Reverse();
            var labels = from item in dateAscending select item.Key;
            var values = from item in dateAscending select -item.TotalOutcome;
            var labelsStr = Newtonsoft.Json.JsonConvert.SerializeObject(labels);
            var valuesStr = Newtonsoft.Json.JsonConvert.SerializeObject(values);

            webView.Eval($"labels = {labelsStr};");
            webView.Eval($"values = {valuesStr};");
            webView.Eval($"RefreshChart();");
        }
    }
}