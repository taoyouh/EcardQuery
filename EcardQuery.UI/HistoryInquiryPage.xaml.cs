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
	public partial class HistoryInquiryPage : MasterDetailPage
	{
		public HistoryInquiryPage ()
		{
            InitializeComponent();
            BindingContext = new HistoryInquiryPageViewModel();
		}
	}

    public class HistoryInquiryPageViewModel : INotifyPropertyChanged
    {
        public HistoryInquiryPageViewModel()
        {
            MasterVM = new HistoryInquiryMasterPageViewModel()
            {
                Accounts = EcardWebsiteHelper.Current.HistoryAccountIds,
                SubmitCommand = new Command(StartInquire),
                StartDate = DateTime.Today.AddTicks(-TimeSpan.TicksPerDay),
                EndDate = DateTime.Today.AddTicks(-TimeSpan.TicksPerDay)
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
            // TODO: 当SelectedAccount为空的时候应当提出警告
            dataGrouper.InputDataCollection.Clear();
            ResultVM.IsRefreshing = true;
            await EcardWebsiteHelper.Current.HistoryInquireAsync(
                MasterVM.StartDate, MasterVM.EndDate,
                MasterVM.SelectedAccount,
                dataGrouper.InputDataCollection);
            ResultVM.IsRefreshing = false;
        }

        ObservableDataGrouper dataGrouper = new ObservableDataGrouper();

        private HistoryInquiryMasterPageViewModel _masterVM;
        public HistoryInquiryMasterPageViewModel MasterVM
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