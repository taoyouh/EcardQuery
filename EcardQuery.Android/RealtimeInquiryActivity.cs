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

namespace EcardQuery
{
    [Activity(Label = "@string/MeCenter_RealtimeInquiry", ParentActivity = typeof(MeCenterActivity))]
    public class RealtimeInquiryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RealtimeInquiry);

            ActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById<Button>(Resource.Id.RealtimeInquiry_InquireButton).Click += inquireButton_Click;
        }

        private void inquireButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RealtimeResultActivity));
            intent.PutExtra("accountId", MainActivity.helper.HistoryAccountIds.First());
            StartActivity(intent);
        }
    }
}