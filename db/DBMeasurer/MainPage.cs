namespace DBMeasurer
{
    using Coding4Fun.Phone.Controls;
    using DBMeasurer.Rules;
    using DBMeasurer.Tools;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.GamerServices;
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using System.Windows.Threading;

    public class MainPage : PhoneApplicationPage
    {
        private bool _contentLoaded;
        private DateTime _matchStartTime;
        private byte[] buffer;
        private double buffMaxDB;
        private double buffMinDB;
        internal Grid ContentPanel;
        private DispatcherTimer dt;
        internal GestureListener Gesture;
        private bool isMatch;
        internal Grid LayoutRoot;
        private MarkList marks;
        private double max_DB;
        private Microphone microphone = Microphone.Default;
        private double min_DB = 200.0;
        internal TextBlock TB_DB;
        internal TextBlock TB_MAXDB;
        internal TextBlock TB_MINDB;
        internal TextBlock TB_Second;
        internal TextBlock TB_Time;
        internal TextBlock TB_TimeOfDay;
        internal StackPanel TimePanel;

        public MainPage()
        {
            this.InitializeComponent();
            this.dt = new DispatcherTimer();
            this.dt.set_Interval(TimeSpan.FromMilliseconds(100.0));
            this.dt.add_Tick(new EventHandler(this, this.dt_Tick));
            this.microphone.add_BufferReady(new EventHandler<EventArgs>(this, this.microphone_BufferReady));
            this.startMeasure();
            this.marks = App.marks;
        }

        private void ApplicationBarIconButton_Click_Start(object sender, EventArgs e)
        {
            if (!this.isMatch)
            {
                MessageBox.Show("你将有15秒的时间，喊出你最大的分贝！！！");
                this.isMatch = true;
                this._matchStartTime = DateTime.get_Now();
                this.buffMinDB = this.min_DB;
                this.buffMaxDB = this.max_DB;
                this.min_DB = 200.0;
                this.max_DB = 0.0;
                this.update();
            }
        }

        private void ApplicationBarIconButton_Click_Stop(object sender, EventArgs e)
        {
            if (this.isMatch)
            {
                this.isMatch = false;
                this.min_DB = this.buffMinDB;
                this.max_DB = this.buffMaxDB;
                this.update();
            }
        }

        private void ApplicationBarMenuItem_Click_Marks(object sender, EventArgs e)
        {
            if (!this.isMatch)
            {
                base.get_NavigationService().Navigate(new Uri("/MarkPage.xaml", 2));
            }
        }

        private void ApplicationBarMenuItem_Click_Question(object sender, EventArgs e)
        {
            if (!this.isMatch)
            {
                EmailComposeTask task = new EmailComposeTask();
                task.set_Body("TIL：\n\t");
                task.set_Subject(string.Format("拼分贝 反馈", new object[0]));
                task.set_To("hkflyor@yahoo.cn");
                task.Show();
            }
        }

        private void ApplicationBarMenuItem_Click_Sett(object sender, EventArgs e)
        {
            if (!this.isMatch)
            {
                base.get_NavigationService().Navigate(new Uri("/SettingPage.xaml", 2));
            }
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            try
            {
                FrameworkDispatcher.Update();
            }
            catch
            {
            }
            this.TB_Time.set_Text(DateTime.get_Now().ToString("hh:mm"));
            this.TB_Second.set_Text(DateTime.get_Now().ToString("ss") ?? "");
            this.TB_TimeOfDay.set_Text(TimeOfDay.GetTimeOfDay(DateTime.get_Now()));
            if (this.isMatch)
            {
                TimeSpan span = DateTime.get_Now() - this._matchStartTime;
                if (span.get_TotalSeconds() > 15.0)
                {
                    if (this.marks.IsMarked(this.max_DB))
                    {
                        this.stopMeasure();
                        InputPrompt prompt = new InputPrompt();
                        prompt.Completed += new EventHandler<PopUpEventArgs<string, PopUpResult>>(this, this.input_Completed);
                        prompt.Title = "请输入你的昵称：";
                        prompt.Message = "本次最高为 " + this.max_DB.ToString("f1") + " dB\n你已经创造了一条记录!";
                        prompt.Show();
                    }
                    else
                    {
                        MessageBox.Show("本次最高为 " + this.max_DB.ToString("f1") + " dB");
                        this.min_DB = this.buffMinDB;
                        this.max_DB = this.buffMaxDB;
                        this.isMatch = false;
                        this.update();
                    }
                }
            }
        }

        private void Gesture_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
        }

        private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            if (e.Direction == null)
            {
                if (e.VerticalChange > 10.0)
                {
                    this.ReduceOpacity(e.VerticalChange);
                }
                if (e.VerticalChange < -10.0)
                {
                    this.IncreaseOpacity(e.VerticalChange);
                }
            }
        }

        private void IncreaseOpacity(double factor)
        {
            double num = Math.Abs(factor * 0.001);
            this.LayoutRoot.set_Opacity(Math.Min(this.LayoutRoot.get_Opacity() + num, 1.0));
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/DBMeasurer;component/MainPage.xaml", 2));
                this.LayoutRoot = base.FindName("LayoutRoot");
                this.Gesture = base.FindName("Gesture");
                this.TimePanel = base.FindName("TimePanel");
                this.TB_Time = base.FindName("TB_Time");
                this.TB_TimeOfDay = base.FindName("TB_TimeOfDay");
                this.TB_Second = base.FindName("TB_Second");
                this.ContentPanel = base.FindName("ContentPanel");
                this.TB_DB = base.FindName("TB_DB");
                this.TB_MINDB = base.FindName("TB_MINDB");
                this.TB_MAXDB = base.FindName("TB_MAXDB");
            }
        }

        private void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if ((e.Error == null) && (e.PopUpResult == ((PopUpResult) PopUpResult.Ok)))
            {
                this.marks.AddMark(new MarkItem { Name = e.Result, MarkOfDB = this.max_DB, MarkedTime = DateTime.get_Now() });
                this.marks.Save();
            }
            this.min_DB = this.buffMinDB;
            this.max_DB = this.buffMaxDB;
            this.isMatch = false;
            this.update();
            this.startMeasure();
        }

        private void microphone_BufferReady(object sender, EventArgs e)
        {
            uint lowerPowerOfTwo = FreqAnalyzer.GetLowerPowerOfTwo(this.microphone.GetData(this.buffer));
            double[] input = new double[lowerPowerOfTwo / 2];
            short[] numArray2 = new short[lowerPowerOfTwo / 2];
            Buffer.BlockCopy(this.buffer, 0, numArray2, 0, lowerPowerOfTwo / 2);
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
                double num7 = num5 / ((double) complexArray.Length);
                if (num7 <= 200.0)
                {
                    if (this.max_DB < num7)
                    {
                        this.max_DB = num7;
                        this.TB_MAXDB.set_Text(this.max_DB.ToString("f1"));
                    }
                    if (this.min_DB > num7)
                    {
                        this.min_DB = num7;
                        this.TB_MINDB.set_Text(this.min_DB.ToString("f1"));
                    }
                    this.TB_DB.set_Text(string.Format("{0} ", num7.ToString("f1")));
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Guide.set_IsScreenSaverEnabled(true);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.skin.IsLight)
            {
                Guide.set_IsScreenSaverEnabled(false);
                if (PhoneApplicationService.get_Current().get_ApplicationIdleDetectionMode() == 1)
                {
                    PhoneApplicationService.get_Current().set_ApplicationIdleDetectionMode(0);
                }
            }
            base.set_Foreground(new SolidColorBrush(App.skin.FontColor));
        }

        private void ReduceOpacity(double factor)
        {
            double num = factor * 0.001;
            this.LayoutRoot.set_Opacity(Math.Max(this.LayoutRoot.get_Opacity() - num, 0.04));
        }

        private void startMeasure()
        {
            if (!this.dt.get_IsEnabled())
            {
                this.dt.Start();
            }
            if (this.microphone.State == MicrophoneState.Stopped)
            {
                this.microphone.set_BufferDuration(TimeSpan.FromMilliseconds(260.0));
                int sampleSizeInBytes = this.microphone.GetSampleSizeInBytes(this.microphone.get_BufferDuration());
                this.buffer = new byte[sampleSizeInBytes];
                this.microphone.Start();
            }
        }

        private void stopMeasure()
        {
            if (this.microphone.State == MicrophoneState.Started)
            {
                this.microphone.Stop();
            }
            if (this.dt.get_IsEnabled())
            {
                this.dt.Stop();
            }
        }

        public void update()
        {
            this.TB_MAXDB.set_Text(this.max_DB.ToString("f1"));
            this.TB_MINDB.set_Text(this.min_DB.ToString("f1"));
        }
    }
}

