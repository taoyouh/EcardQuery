using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcardQuery.UI
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            EcardWebsiteHelper.Current.LoadCookies();
            if (await EcardWebsiteHelper.Current.UpdateLoginState())
            {
                await Navigation.PushAsync(new MainPage());
                Navigation.RemovePage(this);
            }
            else
            {
                loginWebView.Source = "http://ecard.sjtu.edu.cn/shjdportalHome.jsp";
            }
        }

        private async void LoginWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (await EcardWebsiteHelper.Current.UpdateLoginState())
            {
                await Navigation.PushAsync(new MainPage());
                Navigation.RemovePage(this);
            }
        }

        private void RefreshButton_Clicked(object sender, EventArgs e)
        {
            loginWebView.Source = "http://ecard.sjtu.edu.cn/shjdportalHome.jsp";
        }
    }
}