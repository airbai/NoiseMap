namespace DBMeasurer.ViewModel
{
    using DBMeasurer.Rules;
    using System;
    using System.ComponentModel;
    using System.Threading;

    public class MarkesView : MarkItem, INotifyPropertyChanged
    {
        private string _No;
        private PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    PropertyChangedEventHandler handler3 = Delegate.Combine(handler2, value);
                    propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
            remove
            {
                PropertyChangedEventHandler handler2;
                PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                do
                {
                    handler2 = propertyChanged;
                    PropertyChangedEventHandler handler3 = Delegate.Remove(handler2, value);
                    propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, handler3, handler2);
                }
                while (propertyChanged != handler2);
            }
        }

        public MarkesView(MarkItem item, int index)
        {
            this.No = string.Format("NO.{0}", index);
            base.Name = item.Name;
            base.MarkOfDB = item.MarkOfDB;
            base.MarkedTime = item.MarkedTime;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string DB
        {
            get
            {
                return string.Format("{0} dB", base.MarkOfDB.ToString("f2"));
            }
        }

        public string No
        {
            get
            {
                return this._No;
            }
            set
            {
                this._No = value;
                this.NotifyPropertyChanged("No");
            }
        }
    }
}

