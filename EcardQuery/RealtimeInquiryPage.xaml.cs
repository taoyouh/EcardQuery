using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class RealtimeInquiryPage : Page
    {
        public RealtimeInquiryPage()
        {
            this.InitializeComponent();

            accountPicker.DataContext = ((App)(App.Current)).MainWebsiteHelper.HistoryAccountIds;
            accountPicker.SelectedIndex = 0;
        }

        ObservableCollection<TransactionData> dataList = new ObservableCollection<TransactionData>();
        bool isShowingData = false;

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            //progressRing.IsActive = true;
            //controlPanel.IsHitTestVisible = false;
            //controlPanel.Opacity = 0.5;
            try
            {
                //初始化结果面板
                displayList.Hint = "";
                displayList.IsLoading = true;
                dataList = new ObservableCollection<TransactionData>();
                displayList.Data.InputDataCollection = dataList;

                //显示结果面板
                isShowingData = true;
                if (panelStates.CurrentState == controlState)
                {
                    VisualStateManager.GoToState(this, "dataState", true);
                }

                //查询数据
                await ((App)(App.Current)).MainWebsiteHelper.RealtimeInquireAsync((string)accountPicker.SelectedItem, dataList);

                //显示查找结果
                if (dataList.Count == 0 && !displayList.IsLoading)
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
                if (e.NavigationMode == NavigationMode.Back && panelStates.CurrentState == dataState)
                {
                    e.Cancel = true;
                    VisualStateManager.GoToState(this, "controlState", true);
                    isShowingData = false;
                }
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (widthStates.CurrentState == wideState)
                VisualStateManager.GoToState(this, "splitState", false);
            else
            {
                if (isShowingData)
                    VisualStateManager.GoToState(this, "dataState", false);
                else
                    VisualStateManager.GoToState(this, "controlState", false);
            }

            base.OnNavigatedTo(e);
        }

        private void widthStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (widthStates.CurrentState == wideState)
                VisualStateManager.GoToState(this, "splitState", false);
            else
            {
                if (isShowingData)
                    VisualStateManager.GoToState(this, "dataState", true);
                else
                    VisualStateManager.GoToState(this, "controlState", true);
            }
        }
    }
}
