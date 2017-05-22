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
	public partial class RealtimeInquiryPage : ContentPage
	{
        RealTimeInquiryPageViewModel vm;
        ContentPage resultPage;
        bool isInWideState;

        public RealtimeInquiryPage()
        {
            InitializeComponent();
            BindingContext = vm = new RealTimeInquiryPageViewModel();
            vm.ShowResult = ShowResult;
            this.SizeChanged += HistoryInquiryPage_SizeChanged;
        }

        private void HistoryInquiryPage_SizeChanged(object sender, EventArgs e)
        {
            HandleSizeChange(Width);
        }

        private void HandleSizeChange(double width)
        {
            if (width > 600)
            {
                masterColumn.Width = new GridLength(300, GridUnitType.Absolute);
                detailColumn.Width = new GridLength(1, GridUnitType.Star);
                isInWideState = true;
            }
            else
            {
                masterColumn.Width = new GridLength(1, GridUnitType.Star);
                detailColumn.Width = new GridLength(0, GridUnitType.Absolute);
                if (vm.ShowingResult)
                    ShowResult();
                isInWideState = false;
            }
        }

        private void ShowResult()
        {
            if (isInWideState)
            {
                if (Navigation.NavigationStack.Last() == resultPage)
                    Navigation.PopAsync();
            }
            else
            {
                if (resultPage == null)
                {
                    resultPage = new ContentPage
                    {
                        Content = new ResultView(),
                        BindingContext = vm.ResultVM,
                        Title = this.Title
                    };
                }
                if (Navigation.NavigationStack.Last() != resultPage)
                    Navigation.PushAsync(resultPage);
            }
        }
    }

    public class RealTimeInquiryPageViewModel : INotifyPropertyChanged
    {
        public RealTimeInquiryPageViewModel()
        {
            MasterVM = new RealtimeInquiryPanelViewModel()
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
            ShowingResult = false;
        }

        private async void StartInquire()
        {
            dataGrouper.InputDataCollection.Clear();
            ShowResult();
            ShowingResult = true;

            ResultVM.IsRefreshing = true;
            await EcardWebsiteHelper.Current.RealtimeInquireAsync(MasterVM.SelectedAccount, dataGrouper.InputDataCollection);
            ResultVM.IsRefreshing = false;
        }

        ObservableDataGrouper dataGrouper = new ObservableDataGrouper();

        private bool showingResult;
        public bool ShowingResult
        {
            get => showingResult;
            set
            {
                showingResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowingResult)));
            }
        }

        private RealtimeInquiryPanelViewModel _masterVM;
        public RealtimeInquiryPanelViewModel MasterVM
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

        public Action ShowResult;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}