using System;
using System.Collections.Generic;
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
        public TranscationDisplayList()
        {
            this.InitializeComponent();
        }

        public IEnumerable<object> DataList
        {
            get
            {
                return displayList.DataContext as IEnumerable<object>;
            }
            set
            {
                displayList.DataContext = value;
                if (value.Count() > 0)
                    noDataHint.Visibility = Visibility.Collapsed;
                else
                    noDataHint.Visibility = Visibility.Visible;
            }
        }
    }
}
