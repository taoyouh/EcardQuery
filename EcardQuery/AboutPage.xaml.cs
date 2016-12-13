using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Store;
#if DEBUG
using CurrentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
#endif

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EcardQuery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        const string Donation = "donation";
        const string ProductKey = "EcardQuery_Donate";

        public AboutPage()
        {
            this.InitializeComponent();
            VersionBlock.Text = GetVersionNumber();
        }

        private static string GetVersionNumber()
        {
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                var version = assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
                return version.Version;
            }
            catch (Exception)
            {
                return "未知";
            }
        }

        private async void emailButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto://zhaobang.china@hotmail.com?subject=校园卡查询反馈"));
        }

        private void liscenseButton_DBCS_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Liscenses.LiscensePage_DBCS));
        }

        private void liscenseButton_AppInsights_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Liscenses.LiscencePage_AppInsights));
        }

        private async void sourceCodeButton_DBCS_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Aimeast/Encoding4Silverlight"));
        }

        private async void sourceCodeButton_AppInsights_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Microsoft/ApplicationInsights-Home"));
        }

        private async void donateButton_Click(object sender, RoutedEventArgs e)
        {
            PurchaseResults purchaseResults = await CurrentApp.RequestProductPurchaseAsync(ProductKey);
            switch (purchaseResults.Status)
            {
                case ProductPurchaseStatus.Succeeded:
                    int count = UpdateAndGetDonationCount();
                    await CurrentApp.ReportConsumableFulfillmentAsync(ProductKey, purchaseResults.TransactionId);
                    MessageDialog msgDialog;
                    if (count > 1)
                        msgDialog = new MessageDialog(string.Format("这已经是您的第{0}次打赏了，真是万分感谢！"));
                    else
                        msgDialog = new MessageDialog("感谢您的打赏！您随时可以再次打赏哟！");
                    await msgDialog.ShowAsync();
                    break;
                case ProductPurchaseStatus.NotFulfilled:
                    await CurrentApp.ReportConsumableFulfillmentAsync(ProductKey, purchaseResults.TransactionId);
                    msgDialog = new MessageDialog
                        ("不好意思，您上次的打赏似乎出了点问题，但现在已经解决了。您现在可以再次打赏了。");
                    await msgDialog.ShowAsync();
                    break;
            }
        }

        int UpdateAndGetDonationCount()
        {
            var settings = ApplicationData.Current.RoamingSettings;
            if (!settings.Values.ContainsKey(Donation))
            {
                settings.Values.Add(Donation, 1);
            }
            else
            {
                settings.Values[Donation] = (int)settings.Values[Donation] + 1;
            }
            return (int)settings.Values[Donation];
        }
    }
}
