namespace DBMeasurer
{
    using DBMeasurer.Rules;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Navigation;

    public class App : Application
    {
        private bool _contentLoaded;
        public static MarkList marks;
        private bool phoneApplicationInitialized;
        public static SettRule skin;

        public App()
        {
            base.add_UnhandledException(new EventHandler<ApplicationUnhandledExceptionEventArgs>(this, this.Application_UnhandledException));
            this.InitializeComponent();
            this.InitializePhoneApplication();
            if (Debugger.get_IsAttached())
            {
                Application.get_Current().get_Host().get_Settings().set_EnableFrameRateCounter(true);
                PhoneApplicationService.get_Current().set_UserIdleDetectionMode(1);
            }
            skin = new SettRule();
            marks = MarkList.Load();
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.get_IsAttached())
            {
                Debugger.Break();
            }
        }

        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            if (base.get_RootVisual() != this.RootFrame)
            {
                base.set_RootVisual(this.RootFrame);
            }
            this.RootFrame.remove_Navigated(new NavigatedEventHandler(this, this.CompleteInitializePhoneApplication));
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/DBMeasurer;component/App.xaml", 2));
            }
        }

        private void InitializePhoneApplication()
        {
            if (!this.phoneApplicationInitialized)
            {
                this.RootFrame = new PhoneApplicationFrame();
                this.RootFrame.add_Navigated(new NavigatedEventHandler(this, this.CompleteInitializePhoneApplication));
                this.RootFrame.add_NavigationFailed(new NavigationFailedEventHandler(this, this.RootFrame_NavigationFailed));
                this.phoneApplicationInitialized = true;
            }
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.get_IsAttached())
            {
                Debugger.Break();
            }
        }

        public PhoneApplicationFrame RootFrame { get; private set; }
    }
}

