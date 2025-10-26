using Microsoft.Maui.Controls.Maps;


#if ANDROID
using zaraga.plugins.NativeLocation;
#endif
#if IOS
using zaraga.plugins.NativeLocation;
#endif

namespace GeolocationTester
{
    public partial class MainPage : ContentPage
    {
        private bool _getGeolocation = false;
        private readonly NativeLocationService _locationService;


        public MainPage()
        {
            InitializeComponent();
            _locationService = new();
        }


        private async void btnStart_Clicked(object sender, EventArgs e)
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                // Using GeolocationAccuracy.Medium as a balance between accuracy and power consumption.
                // Developers can adjust this value to High or Low based on their specific requirements.
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Medium);
                if (!Geolocation.IsListeningForeground)
                {
                    var success = await Geolocation.StartListeningForegroundAsync(request);
                }

                _getGeolocation = true;

                Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    Task.Run(async () =>
                    {
                        var geolocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(3)));
                        if (geolocation != null)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                lblLongitudeRecurrent.Text = geolocation.Longitude.ToString();
                                lblLatitudeRecurrent.Text = geolocation.Latitude.ToString();
                                lblAccuracyRecurrent.Text = geolocation.Accuracy.ToString();

                                //var pintoRemove = map.Pins.FirstOrDefault(x => x.Label == "Recurrent Location");
                                //if (pintoRemove != null)
                                //    map.Pins.Remove(pintoRemove);
                                //map.Pins.Add(new Pin() { Location = geolocation, Label = "Recurrent Location", Address = "" });
                            });
                        }


                    });
                    return _getGeolocation;
                });

                _locationService.LocationChanged += _locationService_LocationChanged;
                _locationService.StatusChanged += _locationService_StatusChanged;
                _locationService.Initialize();
            }
            catch (Exception ex)
            {
                _getGeolocation = false;
                // Unable to start listening for location changes
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
        }

        private async void _locationService_StatusChanged(object? sender, string e)
        {
            try
            {
                //await Shell.Current.DisplayAlert("Cambio de estatus", e, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
        }

        private async void _locationService_LocationChanged(object? sender, Location e)
        {
            try
            {

                lblLatitude.Text = e.Latitude.ToString();
                lblLongitude.Text = e.Longitude.ToString();
                lblAccuracy.Text = e.Accuracy.ToString();

                map.Pins.Clear();
                map.Pins.Add(new Pin() { Location = new Location(e.Latitude, e.Longitude), Label = "update Location", Address = "" });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
        }

        private async void btnfinish_Clicked(object sender, EventArgs e)
        {
            try
            {
                _getGeolocation = false;
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                Geolocation.StopListeningForeground();
                string status = "Stopped listening for foreground location updates";

                _locationService.Stop();
                _locationService.LocationChanged -= _locationService_LocationChanged;
                _locationService.StatusChanged -= _locationService_StatusChanged;
            }
            catch (Exception ex)
            {
                // Unable to stop listening for location changes
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
        }

        private async void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            //try
            //{
            //    lblLatitude.Text = e.Location.Latitude.ToString();
            //    lblLongitude.Text = e.Location.Longitude.ToString();
            //    lblAccuracy.Text = e.Location.Accuracy.ToString();

            //    MapSpan mapSpan = MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(0.004));
            //    map.MoveToRegion(mapSpan);

            //    map.Pins.Clear();
            //    map.Pins.Add(new Pin() { Location = e.Location, Label = "Current Location", Address = e.Location.Accuracy.ToString() });
            //}
            //catch (Exception ex)
            //{
            //    await Shell.Current.DisplayAlert("", ex.Message, "OK");
            //}
        }

    }

}
