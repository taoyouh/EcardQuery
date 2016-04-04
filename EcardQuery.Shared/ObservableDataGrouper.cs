using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EcardQuery
{
    public class ObservableDataGrouper
    {
        private ObservableCollection<TransactionGroup> _groupedDataCollection = new ObservableCollection<TransactionGroup>();
        private ObservableCollection<TransactionData> _inputDataCollection = new ObservableCollection<TransactionData>();
        public ObservableCollection<TransactionData> InputDataCollection
        {
            get { return _inputDataCollection; }
            set
            {
                _inputDataCollection.CollectionChanged -= DataList_CollectionChanged;
                _inputDataCollection = value;
                _inputDataCollection.CollectionChanged += DataList_CollectionChanged;
                GetGroupsByDate(_inputDataCollection, _groupedDataCollection);
            }
        }

        public ObservableCollection<TransactionGroup> GroupedDataCollection
        {
            get { return _groupedDataCollection; }
        }

        private void DataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                GetGroupsByDate(_inputDataCollection, _groupedDataCollection);
                return;
            }


            if (e.OldItems != null)
                foreach (TransactionData item in e.OldItems)
                {
                    TransactionGroup group = _groupedDataCollection.First(x => x.Key == item.Date.ToString("D"));
                    try { group.Remove(item); } catch { }
                    if (group.Count == 0) _groupedDataCollection.Remove(group);
                }

            if (e.NewItems != null)
            {
                var query = from TransactionData item in e.NewItems
                            orderby item.DateTime
                            group item by item.Date into g
                            select new { GroupName = g.Key.ToString("D"), Items = g };
                foreach (var g in query)
                {
                    TransactionGroup group = _groupedDataCollection.FirstOrDefault(x => x.Key == g.GroupName.ToString());
                    if (group == null)
                    {
                        group = new TransactionGroup();
                        group.Key = g.GroupName;
                        _groupedDataCollection.Add(group);
                    }
                    foreach (TransactionData item in g.Items)
                    {
                        group.Add(item);
                    }
                }
            }
        }

        static void GetGroupsByDate(IEnumerable<TransactionData> inputData, ObservableCollection<TransactionGroup> outputData)
        {
            outputData.Clear();

            var query = from item in inputData
                        orderby item.DateTime
                        group item by item.DateTime.Date into g
                        select new { GroupName = g.Key.ToString(), Items = g };
            foreach (var g in query)
            {
                TransactionGroup group = new TransactionGroup();
                group.Key = g.GroupName;
                foreach (TransactionData item in g.Items)
                {
                    group.Add(item);
                }
            }
        }
    }
}
