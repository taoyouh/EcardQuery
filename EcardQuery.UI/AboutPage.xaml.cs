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
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
            InitializeComponent();
		}

        private void EmailButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DonateButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LicenceButton_DBCS_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}