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

namespace EcardQuery
{

    [Activity(Label = "@string/Result_Title", ParentActivity = typeof(RealtimeInquiryActivity))]
    public class RealtimeResultActivity : Activity
    {
        ListView mainList;
        string accountId;
        ObservableCollection<TransactionData> datas = new ObservableCollection<TransactionData>();
        ObservableDataGrouper dataGrouper = new ObservableDataGrouper();
        TransactionGroupsAdapter adapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            accountId = Intent.GetStringExtra("accountId");

            SetContentView(Resource.Layout.Result);

            ActionBar.SetDisplayHomeAsUpEnabled(true);

            mainList = FindViewById<ListView>(Resource.Id.Result_MainList);

            dataGrouper.InputDataCollection = datas;
            adapter = new TransactionGroupsAdapter(this, dataGrouper.GroupedDataCollection);
            mainList.Adapter = adapter;

            await MainActivity.helper.RealtimeInquireAsync(accountId, datas);

            FindViewById<ProgressBar>(Resource.Id.Result_ProgressBar).Visibility = ViewStates.Gone;
            if (datas.Count == 0)
                FindViewById<TextView>(Resource.Id.Result_EmptyHint).Visibility = ViewStates.Visible;
        }
    }
}