using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcardQuery.UI
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryInquiryMasterPage : ContentPage
	{
		public HistoryInquiryMasterPage ()
		{
			InitializeComponent ();
		}
	}

    public class HistoryInquiryMasterPageViewModel : INotifyPropertyChanged
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

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}