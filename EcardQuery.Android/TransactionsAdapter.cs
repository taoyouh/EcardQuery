using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EcardQuery
{
    public class TransactionGroupsAdapter : BaseAdapter<TransactionGroup>
    {
        ObservableCollection<TransactionGroup> _data;
        List<TransactionsAdapter> _adapters = new List<TransactionsAdapter>();
        Activity _activity;

        public TransactionGroupsAdapter(Activity activity, ObservableCollection<TransactionGroup> data)
        {
            _activity = activity;
            Data = data;
        }

        public override TransactionGroup this[int position]
        {
            get
            {
                return _data[position];
            }
        }

        public override int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public ObservableCollection<TransactionGroup> Data
        {
            get
            {
                return _data;
            }

            set
            {
                if (_data != null)
                    _data.CollectionChanged -= data_CollectionChanged;
                _data = value;
                _data.CollectionChanged += data_CollectionChanged;

                _adapters.Clear();
                foreach (TransactionGroup item in _data)
                {
                    _adapters.Add(new TransactionsAdapter(_activity, item));
                }

                NotifyDataSetChanged();
            }
        }

        private void data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _adapters.Clear();
                foreach (TransactionGroup item in _data)
                {
                    _adapters.Add(new TransactionsAdapter(_activity, item));
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    _adapters.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
                }

                if (e.NewItems != null)
                {
                    int index = e.NewStartingIndex;
                    foreach (TransactionGroup item in e.NewItems)
                    {
                        _adapters.Insert(index, new TransactionsAdapter(_activity, item));
                        index++;
                    }
                }
            }

            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
                view = _activity.LayoutInflater.Inflate(Resource.Layout.TransactionGroup, null);

            ListView childrenList = view.FindViewById<ListView>
                (Resource.Id.TransactionGroup_ChildrenList);

            TextView titleText = view.FindViewById<TextView>
                (Resource.Id.TransactionGroup_TitleText);

            if (childrenList.Adapter != _adapters[position])
            {
                childrenList.Adapter = _adapters[position];
                titleText.Text = _data[position].Key;
                //setListViewHeightBasedOnChildren(childrenList);
                //_adapters[position].Data.CollectionChanged += (sender, e) =>
                //{
                //    setListViewHeightBasedOnChildren(childrenList);
                //};
            }

            return view;
        }

        //public static void setListViewHeightBasedOnChildren(ListView listView)
        //{
        //    var listAdapter = listView.Adapter;
        //    if (listAdapter == null)
        //    {
        //        // pre-condition
        //        return;
        //    }

        //    int totalHeight = 0;
        //    for (int i = 0; i < listAdapter.Count; i++)
        //    {
        //        View listItem = listAdapter.GetView(i, null, listView);
        //        listItem.Measure(0, 0);
        //        totalHeight += listItem.MeasuredHeight;
        //    }

        //    ViewGroup.LayoutParams param = listView.LayoutParameters;
        //    param.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
        //    listView.LayoutParameters = param;
        //}
    }

    public class TransactionsAdapter : BaseAdapter<TransactionData>
    {
        ObservableCollection<TransactionData> _data;
        Activity _activity;

        public TransactionsAdapter(Activity activity, ObservableCollection<TransactionData> data)
        {
            _activity = activity;
            Data = data;
        }

        public override TransactionData this[int position]
        {
            get
            {
                return _data[position];
            }
        }

        public override int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public ObservableCollection<TransactionData> Data
        {
            get
            {
                return _data;
            }

            set
            {
                if (_data != null)
                    _data.CollectionChanged -= data_CollectionChanged;
                _data = value;
                _data.CollectionChanged += data_CollectionChanged;

                NotifyDataSetChanged();
            }
        }

        private void data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
                view = _activity.LayoutInflater.Inflate(Resource.Layout.TransactionDisplay, null);

            var dataItem = _data[position];
            view.FindViewById<TextView>(Resource.Id.Transaction_BalanceText).Text =
                dataItem.AccountBalance.ToString();
            view.FindViewById<TextView>(Resource.Id.Transaction_DeltaText).Text =
                dataItem.Delta.ToString();
            view.FindViewById<TextView>(Resource.Id.Transaction_SubsystemText).Text =
                dataItem.SubSystem;
            view.FindViewById<TextView>(Resource.Id.Transaction_TimeText).Text =
                dataItem.Time.ToString();

            return view;
        }
    }
}