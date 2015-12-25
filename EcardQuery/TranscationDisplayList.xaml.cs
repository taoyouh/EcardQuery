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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EcardQuery
{
    public sealed partial class TranscationDisplayList : UserControl
    {
        private ObservableCollection<TranscationGroup> _groupedDataList = new ObservableCollection<TranscationGroup>();
        public TranscationDisplayList()
        {
            this.InitializeComponent();
            cvs1.Source = _groupedDataList;
        }

        private ObservableCollection<TranscationData> _dataList = new ObservableCollection<TranscationData>();
        public ObservableCollection<TranscationData> DataList
        {
            get { return _dataList; }
            set
            {
                _dataList.CollectionChanged -= DataList_CollectionChanged;
                _dataList = value;
                _dataList.CollectionChanged += DataList_CollectionChanged;
                GetGroupsByDate(_dataList, _groupedDataList);
            }
        }

        private void DataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                GetGroupsByDate(_dataList, _groupedDataList);
                return;
            }


            if (e.OldItems != null)
                foreach (TranscationData item in e.OldItems)
                {
                    TranscationGroup group = _groupedDataList.First(x => x.Key == item.Date.ToString("D"));
                    try { group.Remove(item); } catch { }
                    if (group.Count == 0) _groupedDataList.Remove(group);
                }

            if (e.NewItems != null)
            {
                var query = from TranscationData item in e.NewItems
                            orderby item.DateTime
                            group item by item.Date into g
                            select new { GroupName = g.Key.ToString("D"), Items = g };
                foreach (var g in query)
                {
                    TranscationGroup group = _groupedDataList.FirstOrDefault(x => x.Key == g.GroupName.ToString());
                    if (group == null)
                    {
                        group = new TranscationGroup();
                        group.Key = g.GroupName;
                        _groupedDataList.Add(group);
                    }
                    foreach (TranscationData item in g.Items)
                    {
                        group.Add(item);
                    }
                }
            }
        }

        public string Hint
        {
            get { return noDataHint.Text; }
            set { noDataHint.Text = value; }
        }

        public bool IsLoading
        {
            get { return progressRing.IsActive; }
            set { progressRing.IsActive = value; }
        }

        static void GetGroupsByDate(IEnumerable<TranscationData> inputData, ObservableCollection<TranscationGroup> outputData)
        {
            outputData.Clear();

            var query = from item in inputData
                        orderby item.DateTime
                        group item by item.DateTime.Date into g
                        select new { GroupName = g.Key.ToString(), Items = g };
            foreach (var g in query)
            {
                TranscationGroup group = new TranscationGroup();
                group.Key = g.GroupName;
                foreach (TranscationData item in g.Items)
                {
                    group.Add(item);
                }
            }
        }
    }

    public class ColorConverter : Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal num = (decimal)value;
            if(num>0)
            {
                return new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                return new SolidColorBrush(Windows.UI.Colors.Green);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TranscationGroup : ObservableCollection<TranscationData>
    {
        public string Key { get; set; }
    }
}
