/* 
    Copyright (c) 2010 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
*/
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NM.Common;
using WeiboSdk.PageViews;
using WeiboSdk;
using System.Diagnostics;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using NoiseMap.UserControls;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Navigation;
using Coding4Fun.Phone.Controls;

namespace SilverlightMicrophoneSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Microphone microphone = Microphone.Default;     // Object representing the physical microphone on the device
        private byte[] buffer;                                  // Dynamic buffer to retrieve audio data from the microphone
        private MemoryStream stream = new MemoryStream();       // Stores the audio data for later playback
        private SoundEffectInstance soundInstance;              // Used to play back audio
        private bool soundIsPlaying = false;                    // Flag to monitor the state of sound playback

        // Status images
        private BitmapImage blankImage;
        private BitmapImage microphoneImage;
        private BitmapImage speakerImage;
        private double dbmax;
        private double dbmin;
        GeoCoordinateWatcher watcher;
        Timer Clock;
        bool centerSet = false;
        /// <summary>
        /// Constructor 
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Timer to simulate the XNA Framework game loop (Microphone is 
            // from the XNA Framework). We also use this timer to monitor the 
            // state of audio playback so we can update the UI appropriately.
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(33);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();

            // Event handler for getting audio data when the buffer is full
            microphone.BufferReady += new EventHandler<EventArgs>(microphone_BufferReady);
            this.pivotMain.LoadingPivotItem += new EventHandler<PivotItemEventArgs>(pivotMain_LoadingPivotItem);
            this.googleMapControl.ViewChangeEnd += new EventHandler<MapEventArgs>(googleMapControlHandleViewChangeEnd);

            blankImage = new BitmapImage(new Uri("Images/blank.png", UriKind.RelativeOrAbsolute));
            microphoneImage = new BitmapImage(new Uri("Images/microphone.png", UriKind.RelativeOrAbsolute));
            speakerImage = new BitmapImage(new Uri("Images/speaker.png", UriKind.RelativeOrAbsolute));


            // 此处使用自己 AppKey 和 AppSecret，未经
            //审核的应用只支持用申请该Appkey的帐号来获取数据
            SdkData.AppKey = "187210693";
            SdkData.AppSecret = "024942767c18021d0ed9febd5863aa20";

            // 您app设置的重定向页,必须一致
            SdkData.RedirectUri = "http://1diantao.sinaapp.com/index.php/mobilemap";
            //SdkData.RedirectUri = "https://api.weibo.com/oauth2/default.html";
            App.AccessToken = null;
            App.RefleshToken = null;
        }
        

        private void TimerProc(object state)
        {
            // The state object is the Timer object.
            Timer t = (Timer)state;
            t.Dispose();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("AccessToken");// as string; //App.AccessToken;
            });
        }

        /// <summary>
        /// Updates the XNA FrameworkDispatcher and checks to see if a sound is playing.
        /// If sound has stopped playing, it updates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dt_Tick(object sender, EventArgs e)
        {
            try { FrameworkDispatcher.Update(); }
            catch { }

            if (true == soundIsPlaying)
            {
                if (soundInstance.State != SoundState.Playing)
                {
                    // Audio has finished playing
                    soundIsPlaying = false;

                    // Update the UI to reflect that the 
                    // sound has stopped playing
                    SetButtonStates(true, true, false, true);
                    //StatusImage.Source = blankImage;
                }
            }
        }

        /// <summary>
        /// The Microphone.BufferReady event handler.
        /// Gets the audio data from the microphone and stores it in a buffer,
        /// then writes that buffer to a stream for later playback.
        /// Any action in this event handler should be quick!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void microphone_BufferReady(object sender, EventArgs e)
        {
            double db = 0.0;
            //// Store the audio data in a stream
            //stream.Write(buffer, 0, buffer.Length);
            uint min = FreqAnalyzer.GetLowerPowerOfTwo((uint)this.microphone.GetData(this.buffer));
            double[] input = new double[min / 2];
            short[] numArray2 = new short[min / 2];
            Buffer.BlockCopy(this.buffer, 0, numArray2, 0, (int)(min / 2));
            int num3 = 0;
            short[] numArray3 = numArray2;
            for (int i = 0; i < numArray3.Length; i++)
            {
                float num4 = numArray3[i];
                input[num3++] = num4;
            }
            Complex[] complexArray = FreqAnalyzer.FFT(input, true);
            double num5 = 0.0;
            for (int j = 0; j < complexArray.Length; j++)
            {
                num5 += 20.0 * Math.Log10(Math.Abs(complexArray[j].ToModul()));
            }
            if (num5 >= 0.0)
            {
                db = num5 / ((double)complexArray.Length);
                if (db <= 200.0)
                {
                    if (this.dbmax < db)
                    {
                        this.dbmax = db;
                        this.txtDB.Text = this.dbmax.ToString("f2");
                    }
                    if (this.dbmin > db)
                    {
                        this.dbmin = db;
                        this.txtDB.Text = this.dbmin.ToString("f2");
                    }
                    this.txtDB.Text = string.Format("{0} ", db.ToString("f2"));
                }
            }
            App.CurrentDecibel = db;
        }

        /// <summary>
        /// Handles the Click event for the record button.
        /// Sets up the microphone and data buffers to collect audio data,
        /// then starts the microphone. Also, updates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordButton_Click(object sender, EventArgs e)
        {
            // Get audio data in 1/2 second chunks
            microphone.BufferDuration = TimeSpan.FromMilliseconds(500);

            // Allocate memory to hold the audio data
            buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];

            // Set the stream back to zero in case there is already something in it
            stream.SetLength(0);

            // Start recording
            microphone.Start();

            SetButtonStates(false, false, true, true);
            //StatusImage.Source = microphoneImage;
        }

        /// <summary>
        /// Handles the Click event for the stop button.
        /// Stops the microphone from collecting audio and updates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            if (microphone.State == MicrophoneState.Started)
            {
                // In RECORD mode, user clicked the 
                // stop button to end recording
                microphone.Stop();
            }
            else if (soundInstance.State == SoundState.Playing)
            {
                // In PLAY mode, user clicked the 
                // stop button to end playing back
                soundInstance.Stop();
            }

            SetButtonStates(true, true, false, true);
            //StatusImage.Source = blankImage;
        }

        /// <summary>
        /// Handles the Click event for the play button.
        /// Plays the audio collected from the microphone and updates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, EventArgs e)
        {
            if (stream.Length > 0)
            {
                // Update the UI to reflect that
                // sound is playing
                SetButtonStates(false, false, true, true);
                //StatusImage.Source = speakerImage;

                // Play the audio in a new thread so the UI can update.
                Thread soundThread = new Thread(new ThreadStart(playSound));
                soundThread.Start();
            }
        }

        private void markerButton_Click(object sender, EventArgs e)
        {
            bool enableLocationIsChecked = (bool?)IsolatedStorageSettings.ApplicationSettings["EnableLocation"] ?? false;
            if (!enableLocationIsChecked)
            {
                MessageBox.Show("无法获取您的位置信息。请到手机系统的【设置】->【定位服务】和噪音地图->【设置】中打开并允许使用位置信息。");

                return;
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var url = string.Format("http://2.1diantao.sinaapp.com/index.php/pages/insertdb/{0}/{1}/{2}",
                        App.Latitude,
                        App.Longitude,
                        App.CurrentDecibel);
                    var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                    request.Method = "GET";
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    MessageBox.Show("发送成功");
                                });
                            }
                        }
                        catch
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                MessageBox.Show("出错了，可能是wifi或者gprs没打开");
                            });
                        }

                    }, null);

                }
                catch
                {
                    MessageBox.Show("出错了，可能是wifi或者gprs没打开");
                }
            });
        }
        /// <summary>
        /// Handles the Click event for the play button.
        /// Plays the audio collected from the microphone and updates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shareButton_Click(object sender, EventArgs e)
        {
            //if (stream.Length > 0)
            //{
            // Update the UI to reflect that
            // sound is playing
            //SetButtonStates(false, false, true, true);
            //UserHelp.Text = "share";
            //StatusImage.Source = speakerImage;

            // Play the audio in a new thread so the UI can update.
            //Thread soundThread = new Thread(new ThreadStart(playSound));
            //soundThread.Start();
            string accessToken = string.Empty;
            DateTime dt;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //IsolatedStorageSettings.ApplicationSettings.TryGetValue("AccessToken", out accessToken);// as string; //App.AccessToken;
                //IsolatedStorageSettings.ApplicationSettings.TryGetValue("AccessTokenSetTime", out dt);
                //if (dt == null || DateTime.Compare(dt.AddDays(6), DateTime.Now) < 0 || string.IsNullOrEmpty(accessToken))
                //{
                    if (string.IsNullOrEmpty(SdkData.AppKey) || string.IsNullOrEmpty(SdkData.AppSecret)
                        || string.IsNullOrEmpty(SdkData.RedirectUri))
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("请在中MainPage.xmal.cs的构造函数中设置自己的appkey、appkeysecret、RedirectUri.");
                        });
                        return;
                    }

                    AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);
                    AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);
                    //其它通知事件...

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml"
                            , UriKind.Relative));
                    });
                //}
                //else
                //{
                //    //NavigationService.Navigate(new Uri("/PageViews/GrabInfoToShare.xaml",
                //    //    UriKind.Relative));
                //    this.Share();
                //}
            });
        }

        private void StartGeo()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();

                this.googleMapControl.DefaultCenter = watcher.Position.Location;
            }
        }

        /// <summary>
        /// Plays the audio using SoundEffectInstance 
        /// so we can monitor the playback status.
        /// </summary>
        private void playSound()
        {
            // Play audio using SoundEffectInstance so we can monitor it's State 
            // and update the UI in the dt_Tick handler when it is done playing.
            SoundEffect sound = new SoundEffect(stream.ToArray(), microphone.SampleRate, AudioChannels.Mono);
            soundInstance = sound.CreateInstance();
            soundIsPlaying = true;
            soundInstance.Play();
        }

        /// <summary>
        /// Helper method to change the IsEnabled property for the ApplicationBarIconButtons.
        /// </summary>
        /// <param name="recordEnabled">New state for the record button.</param>
        /// <param name="playEnabled">New state for the play button.</param>
        /// <param name="stopEnabled">New state for the stop button.</param>
        private void SetButtonStates(bool recordEnabled, bool playEnabled, bool stopEnabled, bool markerEnabled)
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = recordEnabled;
            //(ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = stopEnabled;
            //(ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = markerEnabled;

            if (IsolatedStorageSettings.ApplicationSettings.Contains("EnableLocation"))
            {
                bool enableLocationIsChecked = (bool?)IsolatedStorageSettings.ApplicationSettings["EnableLocation"] ?? true;
                if (enableLocationIsChecked)
                {
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = playEnabled;
                }
                else
                {
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                }
            }
            else
            {
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            }
        }

        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {

            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    //IsolatedStorageSettings.ApplicationSettings.Add("AccessToken", response.accesssToken);
                    //IsolatedStorageSettings.ApplicationSettings.Add("AccessTokenSetTime", DateTime.Now);

                    App.AccessToken = response.accesssToken;
                    App.RefleshToken = response.refleshToken;

                    //Clock.Change((long.Parse(response.expriesIn) - 10), 3000);
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.Share();
                });
            }
            else if (errCode.errCode == SdkErrCode.NET_UNUSUAL)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("检查网络");
                });
            }
            else if (errCode.errCode == SdkErrCode.SERVER_ERR)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("服务器返回错误，错误代码:" + errCode.specificCode);
                });
            }
            else
                Debug.WriteLine("Other Err.");

        }

        void Share()
        {
            string message = "我这都" + App.CurrentDecibel.ToString("f2") + "分贝了，赶紧去#噪音地图#（http://1diantao.sinaapp.com）看看你周围多少分贝！";
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
            {
                MessageBox.Show("发送成功");
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                 {
                     NavigationService.Navigate(new Uri("/MainPage.xaml"
                                 , UriKind.Relative));
                 });
            }
            else
                MessageBox.Show(e.Response, e.ErrorCode.ToString(), MessageBoxButton.OK);
        }

        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml"
                            , UriKind.Relative));
            });
        }

        // Event handler for the GeoCoordinateWatcher.StatusChanged event.
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        //statusTextBlock.Text = "you have this application access to location.";
                    }
                    else
                    {
                        //statusTextBlock.Text = "location is not functioning on this device";
                    }
                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    //startLocationButton.IsEnabled = false;
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is not available.";
                    //stopLocationButton.IsEnabled = true;
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is available.";
                    //stopLocationButton.IsEnabled = true;
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            App.Latitude = e.Position.Location.Latitude;
            App.Longitude = e.Position.Location.Longitude;

            if (!centerSet)
            {
                this.googleMapControl.DefaultCenter = e.Position.Location;
                centerSet = true;
            }
        }

        private void recoder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (microphone.State == MicrophoneState.Stopped)
            {
                // Get audio data in 1/2 second chunks
                microphone.BufferDuration = TimeSpan.FromMilliseconds(500);

                // Allocate memory to hold the audio data
                buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];

                // Set the stream back to zero in case there is already something in it
                stream.SetLength(0);

                // Start recording
                microphone.Start();

                SetButtonStates(true, true, true, true);
                UserHelp.Text = "测量中...";

                ImageBrush img = new ImageBrush();
                img.ImageSource = speakerImage;
                this.recoder.Background = img;
            }
            else
            {
                microphone.Stop();
                ImageBrush img = new ImageBrush();
                img.ImageSource = microphoneImage;
                this.recoder.Background = img;
                SetButtonStates(true, true, true, true);
                UserHelp.Text = "点击话筒开始测分贝";
            }
        }

        private void pivotMain_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item.TabIndex == 0)
            {
                ApplicationBar.IsVisible = true;

                //this.SetButtonStates(true, true, true, true);
            }
            else if(e.Item.TabIndex == 1)
            {
                ApplicationBar.IsVisible = false;
                this.GetGeoCoordinates(this.googleMapControl.BoundingRectangle);
            }
        }

        private void googleMapControlHandleViewChangeEnd(object sender, MapEventArgs mapEventArgs)
        {
            if (pivotMain.SelectedIndex == 1)
            {
                this.GetGeoCoordinates(this.googleMapControl.BoundingRectangle);
            }
        }

        private void GetGeoCoordinates(LocationRect boundingRectangle)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var url = string.Format("http://2.1diantao.sinaapp.com/index.php/pages/getdb/{0}",
                    string.Format("{0}/{1}/{2}/{3}",
                    boundingRectangle.East,
                    boundingRectangle.South,
                    boundingRectangle.West,
                    boundingRectangle.North));
                var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.Method = "GET";

                request.BeginGetResponse(a =>
                {
                    try
                    {
                        var response = request.EndGetResponse(a);
                        var responseStream = response.GetResponseStream();
                        using (var sr = new StreamReader(responseStream))
                        {
                            string json = sr.ReadToEnd();
                            Collection<Marker> markers = JsonConvert.DeserializeObject<Collection<Marker>>(json);

                            this.googleMapControl.Pushpins(markers);

                        }
                    }
                    catch(Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("出错了，可能是wifi或者gprs没打开");
                        });

                        throw ex;
                    }
                }, null);


            });
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/PageViews/Settings.xaml"
                    , UriKind.Relative));
            });
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show("您确定要退出噪音地图？", string.Empty, MessageBoxButton.OKCancel);

            if (mb != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Clock = new Timer(new TimerCallback(TimerProc));
            if (IsolatedStorageSettings.ApplicationSettings.Contains("EnableLocation"))
            {
                bool enableLocationIsChecked = (bool?)IsolatedStorageSettings.ApplicationSettings["EnableLocation"] ?? true;
                if (enableLocationIsChecked)
                {
                    this.StartGeo();
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        MessagePrompt mp = new MessagePrompt
                        {
                            BorderThickness= new Thickness(0,0,0,0),
                            Title = "定位服务",
                            IsCancelVisible = true
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

                        var btnHide = new Button { Content = "取消" };
                        btnHide.Click += (o, args) =>
                        {
                            mp.Hide();
                            IsolatedStorageSettings.ApplicationSettings["EnableLocation"] = false;
                        };

                        var btnComplete = new Button { Content = "确认" };
                        btnComplete.Click += (o, args) =>
                        {
                            mp.Hide();
                            IsolatedStorageSettings.ApplicationSettings["EnableLocation"] = true;
                            this.StartGeo();
                        };

                        mp.ActionPopUpButtons.Clear();
                        mp.ActionPopUpButtons.Add(btnHide);
                        mp.ActionPopUpButtons.Add(btnComplete);

                        mp.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OKCancel);
                    }
                });
            }

            this.googleMapControl.DefaultZoomLevel = 14;
        }
    }
}