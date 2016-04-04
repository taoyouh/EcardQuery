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
    [Activity(Label = "@string/MeCenter_HistoryInquiry", ParentActivity = typeof(MeCenterActivity))]
    public class HistoryInquiryActivity : Activity
    {
        TextView startDateBlock;
        TextView endDateBlock;

        DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                _startDate = value;
                startDateBlock.Text = _startDate.ToLongDateString();
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                _endDate = value;
                endDateBlock.Text = _endDate.ToLongDateString();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.HistoryInquiry);

            ActionBar.SetDisplayHomeAsUpEnabled(true);

            startDateBlock = FindViewById<TextView>(Resource.Id.HistoryInquiry_StartDateBlock);
            endDateBlock = FindViewById<TextView>(Resource.Id.HistoryInquiry_EndDateBlock);
            FindViewById<Button>(Resource.Id.HistoryInquiry_StartDateButton).Click += startDateButton_Click;
            FindViewById<Button>(Resource.Id.HistoryInquiry_EndDateButton).Click += endDateButton_Click;
            FindViewById<Button>(Resource.Id.HistoryInquiry_InquireButton).Click += inquireButton_Click;

            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }

        private void startDateButton_Click(object sender, EventArgs e)
        {
            DatePickerFragment dateFragment = new DatePickerFragment(
                x => { StartDate = x; }, StartDate);

            dateFragment.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void endDateButton_Click(object sender, EventArgs e)
        {
            DatePickerFragment dateFragment = new DatePickerFragment(
                x => { EndDate = x; }, EndDate);

            dateFragment.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void inquireButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(HistoryResultActivity));
            intent.PutExtra("startDate", StartDate.Ticks);
            intent.PutExtra("endDate", EndDate.Ticks);
            intent.PutExtra("accountId", MainActivity.helper.HistoryAccountIds.First());
            StartActivity(intent);
        }
    }
}