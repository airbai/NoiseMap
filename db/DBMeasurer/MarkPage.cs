namespace DBMeasurer
{
    using DBMeasurer.ViewModel;
    using Microsoft.Phone.Controls;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class MarkPage : PhoneApplicationPage
    {
        private bool _contentLoaded;
        internal TextBlock ApplicationTitle;
        internal Grid ContentPanel;
        internal Grid LayoutRoot;
        internal ListBox listBoxMarks;
        internal TextBlock PageTitle;
        internal StackPanel TitlePanel;

        public MarkPage()
        {
            this.InitializeComponent();
            lock (App.marks.CurrentMarkList)
            {
                List<MarkesView> list = new List<MarkesView>();
                for (int i = 0; i < App.marks.CurrentMarkList.get_Count(); i++)
                {
                    list.Add(new MarkesView(App.marks.CurrentMarkList.get_Item(i), i + 1));
                }
                this.listBoxMarks.set_ItemsSource(list);
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/DBMeasurer;component/MarkPage.xaml", 2));
                this.LayoutRoot = base.FindName("LayoutRoot");
                this.TitlePanel = base.FindName("TitlePanel");
                this.ApplicationTitle = base.FindName("ApplicationTitle");
                this.PageTitle = base.FindName("PageTitle");
                this.ContentPanel = base.FindName("ContentPanel");
                this.listBoxMarks = base.FindName("listBoxMarks");
            }
        }
    }
}

