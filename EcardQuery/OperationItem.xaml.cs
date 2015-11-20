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
    public sealed partial class OperationItem : UserControl
    {
        public OperationItem()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get
            {
                return titleBlock.Text;
            }
            set
            {
                titleBlock.Text = value;
            }
        }

        public string Description
        {
            get
            {
                return descriptionBlock.Text;
            }
            set
            {
                descriptionBlock.Text = value;
            }
        }
    }
}
