using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                var version = assembly.FullName.Substring(assembly.FullName.IndexOf("Version=") + 8);
                version = version.Substring(0, version.IndexOf(','));
                VersionBlock.Text = version;
            }
            catch(Exception)
            {
                VersionBlock.Text = "未知版本";
            }

        }

        private void EmailButton_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("抱歉", "本功能正在开发中", "关闭");
        }

        private void DonateButton_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("抱歉", "本功能正在开发中", "关闭");
        }

        private void LicenceButton_DBCS_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("抱歉", "本功能正在开发中", "关闭");
        }
    }
}