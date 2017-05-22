using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EcardQuery.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultView : ContentView
    {
        public ResultView()
        {
            InitializeComponent();
        }
    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal num = (decimal)value;
            if (num > 0)
            {
                return Color.Red;
            }
            else
            {
                return Color.Green;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime input)
            {
                return input.ToString("T", culture);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }

    public class ResultPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TransactionGroup> _resultItems;
        public ObservableCollection<TransactionGroup> ResultItems
        {
            get { return _resultItems; }
            set
            {
                _resultItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultItems)));
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRefreshing)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}