using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#endif

namespace EcardQuery
{
    public class TransactionGroup : ObservableCollection<TransactionData>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
            base.OnPropertyChanged(e);
        }

        public string Key { get; set; }

        private decimal _totalIncome = 0;
        public decimal TotalIncome
        {
            get { return _totalIncome; }
            private set
            {
                _totalIncome = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalIncome"));
                    PropertyChanged(this, new PropertyChangedEventArgs("X3"));
                    PropertyChanged(this, new PropertyChangedEventArgs("X4"));
                }
            }
        }

        private decimal _totalOutcome = 0;
        public decimal TotalOutcome
        {
            get { return _totalOutcome; }
            private set
            {
                _totalOutcome = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalOutcome"));
                    PropertyChanged(this, new PropertyChangedEventArgs("X1"));
                    PropertyChanged(this, new PropertyChangedEventArgs("X2"));
                }
            }
        }

#if WINDOWS_UWP
        const int MAXAMOUNT = 50;
        public GridLength X1
        {
            get
            {
                double outcome = -(double)TotalOutcome;
                if (outcome < MAXAMOUNT)
                    return new GridLength(outcome / MAXAMOUNT, GridUnitType.Star);
                else
                    return new GridLength(1, GridUnitType.Star);
            }
        }

        public GridLength X2
        {
            get
            {
                double outcome = -(double)TotalOutcome;
                if (outcome < MAXAMOUNT)
                    return new GridLength(1 - outcome / MAXAMOUNT, GridUnitType.Star);
                else
                    return new GridLength();
            }
        }

        public GridLength X3
        {
            get
            {
                if (TotalIncome < MAXAMOUNT)
                    return new GridLength((double)TotalIncome / MAXAMOUNT, GridUnitType.Star);
                else
                    return new GridLength(1, GridUnitType.Star);
            }
        }

        public GridLength X4
        {
            get
            {
                if (TotalIncome < MAXAMOUNT)
                    return new GridLength(1 - (double)TotalIncome / MAXAMOUNT, GridUnitType.Star);
                else
                    return new GridLength();
            }
        }
#endif

        protected override void InsertItem(int index, TransactionData item)
        {
            if (item.Delta > 0)
                TotalIncome += item.Delta;
            else
                TotalOutcome += item.Delta;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (this[index].Delta > 0)
                TotalIncome += this[index].Delta;
            else
                TotalOutcome += this[index].Delta;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            TotalIncome = 0;
            TotalOutcome = 0;
            base.ClearItems();
        }

        protected override void SetItem(int index, TransactionData item)
        {
            if (this[index].Delta > 0)
                TotalIncome -= this[index].Delta;
            else
                TotalOutcome -= this[index].Delta;

            if (item.Delta > 0)
                TotalIncome += item.Delta;
            else
                TotalOutcome += item.Delta;

            base.SetItem(index, item);
        }
    }
}
