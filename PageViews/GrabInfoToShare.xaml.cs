using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using WeiboSdk;
using Microsoft.Phone.Shell;

namespace SilverlightMicrophoneSample
{
    public partial class GrabInfoToShare : PhoneApplicationPage
    {

        public GrabInfoToShare()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            string message = App.CurrentDecibel.ToString("f2");
            SdkShare sdkShare = new SdkShare
            {
                //设置OAuth2.0的access_token
                AccessToken = App.AccessToken,
                //AccessTokenSecret = App.AccessTokenSecret,

                //PicturePath = "TempJPEG.jpg",
                Message = string.IsNullOrEmpty(message) ? "重新测一下" : message,
                Latitude = App.Latitude.ToString(),
                Longitude = App.Longitude.ToString()

            };

            sdkShare.Completed = new EventHandler<SendCompletedEventArgs>(ShareCompleted);

            //show it
            sdkShare.Show();
        }

        void ShareCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.IsSendSuccess)
                MessageBox.Show("发送成功");
            else
                MessageBox.Show(e.Response, e.ErrorCode.ToString(), MessageBoxButton.OK);
        }
    }
}