using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using Coding4Fun.Phone.Controls;

namespace NoiseMap
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("EnableLocation"))
            {
                EnableLocation.IsChecked = true;
            }
            else
            {
                EnableLocation.IsChecked = (bool?)IsolatedStorageSettings.ApplicationSettings["EnableLocation"] ?? true;
            }

            EnableLocation.Content = EnableLocation.IsChecked.Value ? "已开启定位服务" : "已禁止定位服务";
        }

        private void EnableLocation_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["EnableLocation"] = true;
            EnableLocation.Content = "已开启定位服务";
        }

        private void EnableLocation_Unchecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["EnableLocation"] = false;
            EnableLocation.Content = "已禁止定位服务";
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            new EmailComposeTask
            {
                Subject = "噪音地图位置信息隐私",
                Body = "",
                To = "baipengfei@gmail.com"
            }.Show();
        }

        private void Privacy_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessagePrompt mp = new MessagePrompt
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                Title = "定位服务隐私声明"
            };


            var body = new Grid();
            body.Children.Add(new TextBlock
            {
                Text = @"噪音地图将使用Microsoft定位服务来使用你的地理位置信息。
你的地理位置信息将会保存一段时间。如果你不希望使用的地理位置信息，请点击“取消”按钮。你可以随时到“设置”页面中开启或关闭“定位服务”。
在下述情况下噪音地图需要使用你的地理位置信息：
(1)分享到微博
(2)上传到地图。

使用Microsoft位置服务代表你同意Microsoft收集有关你的位置信息，以提供和改进其地图和位置服务。有关详情，请阅读Microsoft隐私声明（http://www.windowsphone.com/zh-cn/how-to/wp7/start/welcome）。"
            ,
                TextWrapping = TextWrapping.Wrap
            });

            mp.Body = body;
            var btnComplete = new Button { Content = "确认" };
            btnComplete.Click += (o, args) =>
            {
                mp.Hide();
            };

            mp.Show();
        }
    }
}