namespace DBMeasurer
{
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class SettingPage : PhoneApplicationPage
    {
        private bool _contentLoaded;
        internal Grid LayoutRoot;
        internal HyperlinkButton PrivacyPolicyButton;
        internal StackPanel SP_Beauty;
        internal StackPanel stackPanel1;
        internal TextBlock textBlock14;
        internal TextBlock textBlock5;
        internal TextBlock textBlock6;
        internal ToggleSwitch TS_ForbidLock;

        public SettingPage()
        {
            this.InitializeComponent();
            this.SP_Beauty.set_DataContext(App.skin);
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/DBMeasurer;component/SettingPage.xaml", 2));
                this.LayoutRoot = base.FindName("LayoutRoot");
                this.SP_Beauty = base.FindName("SP_Beauty");
                this.TS_ForbidLock = base.FindName("TS_ForbidLock");
                this.stackPanel1 = base.FindName("stackPanel1");
                this.textBlock14 = base.FindName("textBlock14");
                this.PrivacyPolicyButton = base.FindName("PrivacyPolicyButton");
                this.textBlock5 = base.FindName("textBlock5");
                this.textBlock6 = base.FindName("textBlock6");
            }
        }

        private void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.set_Body("TIL：\n\t");
            task.set_Subject(string.Format("拼分贝 反馈", new object[0]));
            task.set_To("hkflyor@yahoo.cn");
            task.Show();
        }
    }
}

