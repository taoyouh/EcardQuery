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
	public partial class RealtimeInquiryPage : MasterDetailPage
	{
		public RealtimeInquiryPage ()
		{
			InitializeComponent ();
            BindingContext = new RealTimeInquiryPageViewModel();
		}
	}

    public class RealTimeInquiryPageViewModel : INotifyPropertyChanged
    {
        public RealTimeInquiryPageViewModel()
        {
            MasterVM = new RealtimeInquiryMasterPageViewModel()
            {
                Accounts = EcardWebsiteHelper.Current.HistoryAccountIds,
                SubmitCommand = new Command(StartInquire)
            };
            MasterVM.SelectedAccount = MasterVM.Accounts.FirstOrDefault();
            ResultVM = new ResultPageViewModel()
            {
                ResultItems = dataGrouper.GroupedDataCollection,
                IsRefreshing = false
            };
        }

        private async void StartInquire()
        {
            dataGrouper.InputDataCollection.Clear();
            ResultVM.IsRefreshing = true;
            await EcardWebsiteHelper.Current.RealtimeInquireAsync(MasterVM.SelectedAccount, dataGrouper.InputDataCollection);
            ResultVM.IsRefreshing = false;
        }

        ObservableDataGrouper dataGrouper = new ObservableDataGrouper();

        private RealtimeInquiryMasterPageViewModel _masterVM;
        public RealtimeInquiryMasterPageViewModel MasterVM
        {
            get => _masterVM;
            set
            {
                _masterVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MasterVM)));
            }
        }

        private ResultPageViewModel _resultVM;
        public ResultPageViewModel ResultVM
        {
            get => _resultVM;
            set
            {
                _resultVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultVM)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}