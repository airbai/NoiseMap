﻿#pragma checksum "C:\test\WP7\NM\SilverlightMicrophoneSample\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6EA4717D7790D76875717DA57C3EBCF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NoiseMap.UserControls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SilverlightMicrophoneSample {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot pivotMain;
        
        internal System.Windows.Controls.StackPanel MainPanel;
        
        internal System.Windows.Controls.Grid recoder;
        
        internal System.Windows.Controls.TextBlock txtDB;
        
        internal System.Windows.Controls.TextBlock UserHelp;
        
        internal NoiseMap.UserControls.GoogleMapControl googleMapControl;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton shareButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton makerButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem Settings;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/NoiseMap;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.pivotMain = ((Microsoft.Phone.Controls.Pivot)(this.FindName("pivotMain")));
            this.MainPanel = ((System.Windows.Controls.StackPanel)(this.FindName("MainPanel")));
            this.recoder = ((System.Windows.Controls.Grid)(this.FindName("recoder")));
            this.txtDB = ((System.Windows.Controls.TextBlock)(this.FindName("txtDB")));
            this.UserHelp = ((System.Windows.Controls.TextBlock)(this.FindName("UserHelp")));
            this.googleMapControl = ((NoiseMap.UserControls.GoogleMapControl)(this.FindName("googleMapControl")));
            this.shareButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("shareButton")));
            this.makerButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("makerButton")));
            this.Settings = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("Settings")));
        }
    }
}
