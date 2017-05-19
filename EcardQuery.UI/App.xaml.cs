using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
