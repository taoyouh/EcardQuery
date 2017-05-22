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
	public partial class HistoryInquiryPage : ContentPage
	{
        HistoryInquiryPageViewModel vm;
        ContentPage resultPage;
        bool isInWideState;

        public HistoryInquiryPage ()
		{
            InitializeComponent();
            BindingContext = vm = new HistoryInquiryPageViewModel();
            vm.ShowResult = ShowResult;
            vm.DisplayAlertAsync = DisplayAlert;
            this.SizeChanged += HistoryInquiryPage_SizeChanged;
		}

        private void HistoryInquiryPage_SizeChanged(object sender, EventArgs e)
        {
            HandleSizeChange(Width);
        }

        private void HandleSizeChange(double width)
        {
            if (width > 700)
            {
                masterColumn.Width = new GridLength(350, GridUnitType.Absolute);
                detailColumn.Width = new GridLength(1, GridUnitType.Star);
                isInWideState = true;
            }
            else
            {
                masterColumn.Width = new GridLength(1, GridUnitType.Star);
                detailColumn.Width = new GridLength(0, GridUnitType.Absolute);
                isInWideState = false;
            }
            if (vm.ShowingResult)
                ShowResult();
            // TODO:同步滚动位置
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

    public class HistoryInquiryPageViewModel : INotifyPropertyChanged
    {
        public HistoryInquiryPageViewModel()
        {
            MasterVM = new HistoryInquiryPanelViewModel()
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
            ShowingResult = false;
        }

        private async void StartInquire()
        {
            // TODO: 当SelectedAccount为空的时候应当提出警告
            try
            {
                dataGrouper.InputDataCollection.Clear();
                ShowResult();
                ShowingResult = true;

                ResultVM.IsRefreshing = true;
                await EcardWebsiteHelper.Current.HistoryInquireAsync(
                    MasterVM.StartDate, MasterVM.EndDate,
                    MasterVM.SelectedAccount,
                    dataGrouper.InputDataCollection);
            }
            catch(Exception)
            {
                await DisplayAlertAsync("查询失败", "您仍然可以查看已经查询到的部分结果。","关闭");
            }
            ResultVM.IsRefreshing = false;
        }

        ObservableDataGrouper dataGrouper = new ObservableDataGrouper();

        private bool _showingResult;
        public bool ShowingResult
        {
            get => _showingResult;
            set
            {
                _showingResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowingResult)));
            }
        }

        private HistoryInquiryPanelViewModel _masterVM;
        public HistoryInquiryPanelViewModel MasterVM
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

        public delegate Task DisplayAlertDelegate(string title, string message, string cancel);
        public DisplayAlertDelegate DisplayAlertAsync;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}