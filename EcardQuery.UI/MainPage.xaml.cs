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
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
            InitializeComponent();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            balanceBlock.Text = $"￥{(await EcardWebsiteHelper.Current.GetBalanceAsync()).AccountBalance}";
        }

        private void Op_history_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HistoryInquiryPage());
        }

        private void Op_realtime_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RealtimeInquiryPage());
        }

        private void Op_about_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AboutPage());
        }

        private async void Op_logout_Tapped(object sender, EventArgs e)
        {
            EcardWebsiteHelper.Current.Logout();
            await Navigation.PushAsync(new LoginPage());
            while(Navigation.NavigationStack.Count>1)
            {
                Navigation.RemovePage(Navigation.NavigationStack.First());
            }
        }
    }
}