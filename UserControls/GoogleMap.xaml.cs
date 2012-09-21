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
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Collections.ObjectModel;

namespace NoiseMap.UserControls
{
    public partial class GoogleMapControl : UserControl
    {
        public GeoCoordinate DefaultCenter 
        {
            set
            {
                if (value != GeoCoordinate.Unknown)
                {
                    Pushpin locationPushpin = new Pushpin();
                    locationPushpin.Location = value;

                    this.googlemap.Children.Add(locationPushpin);

                    Style SampleStyle = this.Resources["PushpinStyle"] as Style;
                    locationPushpin.Style = SampleStyle;
                    this.googlemap.SetView(value, this.DefaultZoomLevel);
                }
            }
        } 

        public int DefaultZoomLevel { get; set; }

        public LocationRect BoundingRectangle
        {
            get
            {
                return this.googlemap.BoundingRectangle;
            }
        }

        public EventHandler<MapEventArgs> ViewChangeEnd;

        Collection<PushpinModel> pushpinCollection = new Collection<PushpinModel>();
        public Collection<PushpinModel> PushpinCollection
        {
            get
            {
                return this.pushpinCollection;
            }
        }

        public GoogleMapControl()
        {
            InitializeComponent();
            street.Visibility = Visibility.Visible;
            //st.IsChecked = true;
            this.googlemap.ViewChangeEnd += this.HandleViewChangeEnd;
        }

        public void Pushpins(Collection<Marker> markers)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //this.pushpinCollection.Clear();
                Style style = this.Resources["PushpinMarkerStyle"] as Style;
                //foreach (var item in this.googlemap.Children)
                //{
                //    if (item is Pushpin)
                //    {
                //        this.googlemap.Children.Remove(item);
                //    }
                //}
                foreach (Marker item in markers)
                {
                    Pushpin locationPushpin = new Pushpin();
                    locationPushpin.Background = new SolidColorBrush(Colors.Gray);
                    locationPushpin.Content = Math.Round(item.db);
                    locationPushpin.Location = new GeoCoordinate(item.lat, item.lng);

                    this.googlemap.Children.Add(locationPushpin);
                    //locationPushpin.Style = style;

                    //PushpinModel model = new PushpinModel();
                    //model.Content = Math.Round(item.db).ToString();
                    //model.Location = new GeoCoordinate(item.lat, item.lng);

                    //this.pushpinCollection.Add(model);

                }

                //this.pushpins.DataContext = this.pushpinCollection;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (h.IsChecked == true)
            //{
            //    hybrid.Visibility = Visibility.Visible;
            //    satellite.Visibility = Visibility.Collapsed;
            //    street.Visibility = Visibility.Collapsed;
            //    physical.Visibility = Visibility.Collapsed;
            //    wateroverlay.Visibility = Visibility.Collapsed;
            //}
            //else if (st.IsChecked == true)
            //{
            //    hybrid.Visibility = Visibility.Collapsed;
            //    satellite.Visibility = Visibility.Collapsed;
            //    street.Visibility = Visibility.Visible;
            //    physical.Visibility = Visibility.Collapsed;
            //    wateroverlay.Visibility = Visibility.Collapsed;
            //}
            //else if (sl.IsChecked == true)
            //{
            //    hybrid.Visibility = Visibility.Collapsed;
            //    satellite.Visibility = Visibility.Visible;
            //    street.Visibility = Visibility.Collapsed;
            //    physical.Visibility = Visibility.Collapsed;
            //    wateroverlay.Visibility = Visibility.Collapsed;
            //}
            //else if (p.IsChecked == true)
            //{
            //    hybrid.Visibility = Visibility.Collapsed;
            //    satellite.Visibility = Visibility.Collapsed;
            //    street.Visibility = Visibility.Collapsed;
            //    physical.Visibility = Visibility.Visible;
            //    wateroverlay.Visibility = Visibility.Collapsed;

            //}
            //else
            //{
            //    hybrid.Visibility = Visibility.Collapsed;
            //    satellite.Visibility = Visibility.Collapsed;
            //    street.Visibility = Visibility.Collapsed;
            //    physical.Visibility = Visibility.Collapsed;
            //    wateroverlay.Visibility = Visibility.Visible;
            //}
        }

        private void ButtonZoomIn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	googlemap.ZoomLevel++;
        }

        private void ButtonZoomOut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	googlemap.ZoomLevel--;
        }

        private void HandleViewChangeEnd(object sender, MapEventArgs mapEventArgs)
        {
            if (this.ViewChangeEnd != null)
            {
                this.ViewChangeEnd(sender, mapEventArgs);
            }
        }
    }

    public enum GoogleTileTypes
    {
        Hybrid,
        Physical,
        Street,
        Satellite,
        WaterOverlay
    }

    public class GoogleTile : Microsoft.Phone.Controls.Maps.TileSource
    {
        private int _server;
        private char _mapmode;
        private GoogleTileTypes _tiletypes;

        public GoogleTileTypes TileTypes
        {
            get { return _tiletypes; }
            set
            {
                _tiletypes = value;
                MapMode = MapModeConverter(value);
            }
        }

        public char MapMode
        {
            get { return _mapmode; }
            set { _mapmode = value; }
        }

        public int Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public GoogleTile()
        {
            UriFormat = @"http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}&hl=zh-CN";
            Server = 1;
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            if (zoomLevel > 0)
            {
                var Url = string.Format(UriFormat, Server, MapMode, zoomLevel, x, y);
                return new Uri(Url);
            }
            return null;
        }

        private char MapModeConverter(GoogleTileTypes tiletype)
        {
            switch (tiletype)
            {
                case GoogleTileTypes.Hybrid:
                    {
                        return 'y';
                    }
                case GoogleTileTypes.Physical:
                    {
                        return 't';
                    }
                case GoogleTileTypes.Satellite:
                    {
                        return 's';
                    }
                case GoogleTileTypes.Street:
                    {
                        return 'm';
                    }
                case GoogleTileTypes.WaterOverlay:
                    {
                        return 'r';
                    }
            }
            return ' ';
        }
    }

    public class Marker
    {
        public string id { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public double db { get; set; }
    }

    public class PushpinModel
    {
                    public Brush Background = new SolidColorBrush(Colors.Gray);
                    public string Content = string.Empty;
                    public GeoCoordinate Location;
    }
}
