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
using System.Net.Http;

namespace EcardQuery
{
    [Activity(Label = "@string/MeCenter_Title")]
    public class MeCenterActivity : Activity
    {
        TextView balanceText;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MeCenter);

            FindViewById<Button>(Resource.Id.MeCenter_LogOutButton).Click += logOutButton_Click;
            FindViewById<Button>(Resource.Id.MeCenter_HistoryInquiryButton).Click += historyInquiryButton_Click;
            FindViewById<Button>(Resource.Id.MeCenter_RealtimeInquiryButton).Click += realtimeInquiry_Click;

            balanceText = FindViewById<TextView>(Resource.Id.MeCenter_BalanceBlock);

            try
            {
                balanceText.Text = (await MainActivity.helper.GetBalanceAsync()).
                    AccountBalance.ToString("C");
            }
            catch ( HttpRequestException)
            {
                balanceText.Text = GetString(Resource.String.MeCenter_BalanceAcquireFailed);
            }
        }

        private void logOutButton_Click(object sender, EventArgs e)
        {
            MainActivity.helper.Dispose();
            StartActivity(typeof(MainActivity));
            Finish();
        }

        private void historyInquiryButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(HistoryInquiryActivity));
        }


        private void realtimeInquiry_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RealtimeInquiryActivity));
        }
    }
}