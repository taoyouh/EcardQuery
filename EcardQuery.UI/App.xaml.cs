using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace EcardQuery.UI
{
	public partial class App : Application
	{
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
        }

        public App(Page startPage)
        {
            InitializeComponent();
            MainPage = startPage;
        }

        protected override void OnStart ()
		{
            // Handle when your app starts
            MobileCenter.Start("uwp=4de696b4-d7f0-4692-9fdd-9cb902431fd1;" +
                               "android=1893da84-a2b0-4eed-a253-adfb55392870;" +
                               "ios=fb65509f-28f8-43a7-8bc8-c7c3c6526c68",
                   typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override async void OnResume ()
		{
            if (!await EcardWebsiteHelper.Current.UpdateLoginState())
            {
                //TODO: Ask the user to login again
            }
			// Handle when your app resumes
		}
	}
}
