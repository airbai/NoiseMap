namespace DBMeasurer.Rules
{
    using System;
    using System.ComponentModel;
    using System.IO.IsolatedStorage;
    using System.Threading;
    using System.Windows.Media;

    public class SettRule : INotifyPropertyChanged
    {
        public string _BackGroundImage;
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

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string BackGroundImage
        {
            get
            {
                return "123156";
            }
        }

        public Color FontColor
        {
            get
            {
                Color color;
                if (IsolatedStorageSettings.get_ApplicationSettings().TryGetValue<Color>("FontColor", ref color))
                {
                    return color;
                }
                IsolatedStorageSettings.get_ApplicationSettings().set_Item("FontColor", Colors.get_Red());
                IsolatedStorageSettings.get_ApplicationSettings().Save();
                return Colors.get_Red();
            }
            set
            {
                IsolatedStorageSettings.get_ApplicationSettings().set_Item("FontColor", value);
                IsolatedStorageSettings.get_ApplicationSettings().Save();
                this.NotifyPropertyChanged("FontColor");
            }
        }

        public bool IsLight
        {
            get
            {
                bool flag;
                if (IsolatedStorageSettings.get_ApplicationSettings().TryGetValue<bool>("IsLight", ref flag))
                {
                    return flag;
                }
                IsolatedStorageSettings.get_ApplicationSettings().set_Item("IsLight", true);
                IsolatedStorageSettings.get_ApplicationSettings().Save();
                return true;
            }
            set
            {
                IsolatedStorageSettings.get_ApplicationSettings().set_Item("IsLight", value);
                IsolatedStorageSettings.get_ApplicationSettings().Save();
                this.NotifyPropertyChanged("IsLight");
            }
        }
    }
}

