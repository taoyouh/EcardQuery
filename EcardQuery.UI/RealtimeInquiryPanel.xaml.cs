using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcardQuery.UI
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RealtimeInquiryPanel : ContentView
	{
		public RealtimeInquiryPanel ()
		{
            InitializeComponent();
		}
	}

    public class RealtimeInquiryPanelViewModel : INotifyPropertyChanged
    {
        private IEnumerable<string> _accounts;
        public IEnumerable<string> Accounts
        {
            get => _accounts;
            set
            {
                _accounts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Accounts)));
            }
        }

        private string _selectedAccount;
        public string SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAccount)));
            }
        }

        private Command _submitCommand;
        public Command SubmitCommand
        {
            get => _submitCommand;
            set
            {
                _submitCommand = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubmitCommand)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}