using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EcardQuery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
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
    }
}
