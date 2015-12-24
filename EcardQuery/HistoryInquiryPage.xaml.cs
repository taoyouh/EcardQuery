using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
        ObservableCollection<TranscationData> dataList = new ObservableCollection<TranscationData>();
        bool isShowingData = false;

        public HistoryInquiryPage()
        {
            this.InitializeComponent();
            accountPicker.DataContext = ((App)(App.Current)).MainWebsiteHelper.HistoryAccountIds;
            accountPicker.SelectedIndex = 0;

            displayList.DataList = dataList;

            startDatePicker.Date = DateTime.Today - TimeSpan.FromDays(3);
            endDatePicker.Date = DateTime.Today - TimeSpan.FromDays(1);
            startDatePicker.MaxDate = DateTime.Today - TimeSpan.FromDays(1);
            endDatePicker.MaxDate = DateTime.Today - TimeSpan.FromDays(1);
            Application.Current.Resuming += Current_Resuming;
        }

        private void Current_Resuming(object sender, object e)
        {
            startDatePicker.MaxDate = DateTime.Today - TimeSpan.FromDays(1);
            endDatePicker.MaxDate = DateTime.Today - TimeSpan.FromDays(1);
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
                MessageDialog msgDialog = new MessageDialog("必须选择开始日期和结束日期", "未完成");
                await msgDialog.ShowAsync();
            }
        }

        private async System.Threading.Tasks.Task InquireHistoryAsync(string startDate, string endDate)
        {
            //progressRing.IsActive = true;
            //controlPanel.IsHitTestVisible = false;
            //controlPanel.Opacity = 0.5;
            try
            {
                //初始化结果面板
                displayList.Hint = "";
                displayList.IsLoading = true;
                dataList = new ObservableCollection<TranscationData>();
                displayList.DataList = dataList;

                //显示结果面板
                isShowingData = true;
                displayList.Visibility = Visibility.Visible;
                if (ActualWidth < 600)
                {
                    controlPanel.Visibility = Visibility.Collapsed;
                }

                //开始查找
                await ((App)(App.Current)).MainWebsiteHelper.HistoryInquire
                    (startDate, endDate, (string)accountPicker.SelectedItem, dataList);

                //显示查找结果
                if(dataList.Count==0 && !displayList.IsLoading)
                {
                    displayList.Hint = "没有查询到任何的数据";
                }
            }
            catch (Exception ex)
            {
                displayList.Hint = "查询失败：" + ex.GetType().ToString() + "\n" + ex.Message;
            }
            finally
            {
                displayList.IsLoading = false;
            }
            //progressRing.IsActive = false;
            //controlPanel.IsHitTestVisible = true;
            //controlPanel.Opacity = 1;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e != null)
                if (isShowingData && controlPanel.Visibility == Visibility.Collapsed && e.NavigationMode == NavigationMode.Back)
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
