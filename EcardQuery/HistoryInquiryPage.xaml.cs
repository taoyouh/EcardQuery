using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EcardQuery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HistoryInquiryPage : Page
    {
        public static List<TranscationData> dataList;
        bool isShowingData = false;

        public HistoryInquiryPage()
        {
            this.InitializeComponent();
            accountPicker.DataContext = App.websiteHelper.HistoryAccountIds;
            accountPicker.SelectedIndex = 0;
        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (startDatePicker.Date.HasValue && endDatePicker.Date.HasValue)
            {
                string startDate = startDatePicker.Date.Value.Date.ToString("yyyyMMdd");
                string endDate = endDatePicker.Date.Value.Date.ToString("yyyyMMdd");

                await InquireHistoryAsync(startDate, endDate);
            }
            else
            {
                statusBlock.Text = "必须输入开始日期和结束日期。";
            }
        }

        private async System.Threading.Tasks.Task InquireHistoryAsync(string startDate, string endDate)
        {
            progressRing.IsActive = true;
            try
            {
                dataList = await App.websiteHelper.HistoryInquire(startDate, endDate, (string)accountPicker.SelectedItem);
                displayList.DataList = dataList;
                isShowingData = true;
                displayList.Visibility = Visibility.Visible;
                if (ActualWidth < 600)
                {
                    controlPanel.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception ex)
            {
                statusBlock.Text = ex.Message;
            }
            progressRing.IsActive = false;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if(isShowingData &&controlPanel.Visibility==Visibility.Collapsed&& e.NavigationMode==NavigationMode.Back)
            {
                e.Cancel = true;
                controlPanel.Visibility = Visibility.Visible;
                displayList.Visibility = Visibility.Collapsed;
                isShowingData = false;
                return;
            }
            base.OnNavigatingFrom(e);
        }

        private void VisualStateGroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if(e.NewState== wideState)
            {
                controlPanel.Visibility = Visibility.Visible;
                displayList.Visibility = Visibility.Visible;
            }
            else
            {
                if (isShowingData)
                    controlPanel.Visibility = Visibility.Collapsed;
                else
                    displayList.Visibility = Visibility.Collapsed;
            }
        }

        private async void expressButton1_Click(object sender, RoutedEventArgs e)
        {
            string endDate = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            string startDate = DateTime.Today.AddDays(-7).ToString("yyyyMMdd");

            await InquireHistoryAsync(startDate, endDate);
        }

        private async void expressButton2_Click(object sender, RoutedEventArgs e)
        {
            string endDate = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            string startDate = DateTime.Today.AddDays(-30).ToString("yyyyMMdd");

            await InquireHistoryAsync(startDate, endDate);
        }
    }
}
