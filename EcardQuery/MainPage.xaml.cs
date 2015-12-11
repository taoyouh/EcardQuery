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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace EcardQuery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("userName") && localSettings.Values.ContainsKey("passwd"))
            {
                userNameBox.Text = (string)localSettings.Values["userName"];
                passwdBox.Password = (string)localSettings.Values["passwd"];
                forgetMeButton.Visibility = Visibility.Visible;
                rememberMeCheckBox.Visibility = Visibility.Collapsed;
                rememberMeCheckBox.IsChecked = false;
            }
            else
            {
                forgetMeButton.Visibility = Visibility.Collapsed;
                rememberMeCheckBox.Visibility = Visibility.Visible;
            }

            ((App)(App.Current)).MainWebsiteHelper = new EcardWebsiteHelper();
            RefreshCheckPic();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            Frame.BackStack.Clear();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedFrom(e);
        }

        private async void RefreshCheckPic()
        {
            //TODO: 此处应当显示“Loading”的图片
            randLoadingHint.Visibility = Visibility.Visible;
            randImage.Width = 0;
            try { randImage.Source = await ((App)(App.Current)).MainWebsiteHelper.GetCheckPicAsync(); }
            catch (Exception)
            {
                try { randImage.Source = await ((App)(App.Current)).MainWebsiteHelper.GetCheckPicAsync(); }
                catch (Exception)
                {
                    ((App)(App.Current)).MainWebsiteHelper = new EcardWebsiteHelper();
                    try { randImage.Source = await ((App)(App.Current)).MainWebsiteHelper.GetCheckPicAsync(); }
                    catch (Exception ex)
                    {
                        statusBlock.Text += "获取验证码失败：\n"
                            + ex.GetType().ToString() + "\n" + ex.Message;
                        randResetButton.Visibility = Visibility.Visible;
                    }
                }
            }
            finally
            {
                randLoadingHint.Visibility = Visibility.Collapsed;
            }
            randImage.Height = randBox.ActualHeight;
            randImage.Width = double.NaN;
            }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RefreshCheckPic();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            await LoginAsync();
        }

        private async System.Threading.Tasks.Task LoginAsync()
        {
            progressRing.IsActive = true;
            if (rememberMeCheckBox.IsChecked.Value)
            {
                Windows.Storage.ApplicationDataContainer localSettings =
                    Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["userName"] = userNameBox.Text;
                localSettings.Values["passwd"] = passwdBox.Password;
            }
            try
            {
                await ((App)(App.Current)).MainWebsiteHelper.LoginAsync(EcardWebsiteHelper.LoginType.PersonId, userNameBox.Text, passwdBox.Password, randBox.Text);
                statusBlock.Text = "";
                await ((App)(App.Current)).MainWebsiteHelper.HistoryInquiryInit();
                Frame.Navigate(typeof(MeCenterPage));
            }
            catch (Exception ex)
            {
                statusBlock.Text = "登录失败：" + ex.GetType().ToString()
                     + "\n" + ex.Message;
                RefreshCheckPic();
            }
            progressRing.IsActive = false;
        }

        private void userNameBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                passwdBox.Focus(FocusState.Keyboard);
        }

        private void passwdBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                randBox.Focus(FocusState.Keyboard);
        }

        private async void randBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await LoginAsync();
            }
        }

        private void userNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            forgetMeButton.Visibility = Visibility.Collapsed;
            rememberMeCheckBox.Visibility = Visibility.Visible;
        }

        private void passwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            forgetMeButton.Visibility = Visibility.Collapsed;
            rememberMeCheckBox.Visibility = Visibility.Visible;
        }

        private void randResetButton_Click(object sender, RoutedEventArgs e)
        {
            randResetButton.Visibility = Visibility.Collapsed;
            RefreshCheckPic();
        }

        private void forgetMeButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings =
                Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("userName");
            localSettings.Values.Remove("passwd");
            userNameBox.Text = "";
            passwdBox.Password = "";
        }
    }
}
