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
    public sealed class NoScrollListview : ListView
    {
        // Constructors
        public NoScrollListview(Context context) : base(context) { }
        public NoScrollListview(Context context, Android.Util.IAttributeSet attributeSet) : base(context, attributeSet) { }
        public NoScrollListview(Context context, Android.Util.IAttributeSet attributeSet, int defaultStyle) : base(context, attributeSet, defaultStyle) { }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int expandSpec = MeasureSpec.MakeMeasureSpec
                (int.MaxValue >> 2, MeasureSpecMode.AtMost);
            base.OnMeasure(widthMeasureSpec, expandSpec);
        }
    }
}