using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EcardQuery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MeCenterPage : Page
    {
        //TODO: 个人中心应当有余额显示
        public MeCenterPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            Frame.BackStack.Clear();

            try
            {
                balanceBlock.Text = '￥' +
                    (await ((App)(App.Current)).MainWebsiteHelper.GetBalanceAsync()).
                    AccountBalance.ToString();
            }
            catch(Exception ex)
            {
                balanceBlock.Text = "获取余额失败：" + ex.GetType().ToString()
                    + "\n" + ex.Message;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == op_historyInquiry)
                Frame.Navigate(typeof(HistoryInquiryPage));
            else if (e.ClickedItem == op_realTimeInquiry)
                Frame.Navigate(typeof(RealtimeInquiryPage));
            //else if (e.ClickedItem == op_logout)
            //    Frame.Navigate(typeof(MainPage));
            else if (e.ClickedItem == op_about)
                Frame.Navigate(typeof(AboutPage));
        }
    }
}
