using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using zaraga.plugins.NativeLocation;
using zaraga.plugins.NativeLocation.Models;

namespace GeolocationTester
{
    public partial class MainPage : ContentPage
    {
        private bool _getGeolocation = false;
        //private readonly NativeLocationService _locationService;


        public MainPage()
        {
            InitializeComponent();
            //_locationService = new();
        }


        private async void btnStart_Clicked(object sender, EventArgs e)
        {
            try
            {
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

                                var pintoRemove = map.Pins.FirstOrDefault(x => x.Label == "MAUI Location");
                                if (pintoRemove != null)
                                    map.Pins.Remove(pintoRemove);
                                map.Pins.Add(new Pin() { Location = geolocation, Label = "MAUI Location", Address = "" });
                            });
                        }


                    });
                    return _getGeolocation;
                });

                NativeLocationService.Instance.LocationChanged += LocationService_LocationChanged;
                NativeLocationService.Instance.StatusChanged += LocationService_StatusChanged;
                NativeLocationService.Instance.Initialize();
            }
            catch (Exception ex)
            {
                _getGeolocation = false;
                // Unable to start listening for location changes
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
        }

        private async void LocationService_StatusChanged(object? sender, string e)
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

        private async void LocationService_LocationChanged(object? sender, NativeLocationModel e)
        {
            try
            {

                lblLatitude.Text = e.Latitude.ToString();
                lblLongitude.Text = e.Longitude.ToString();
                lblAccuracy.Text = e.Accuracy.ToString();

                MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Location(e.Latitude, e.Longitude), Distance.FromKilometers(0.004));
                map.MoveToRegion(mapSpan);

                var pintoRemove = map.Pins.FirstOrDefault(x => x.Label == "Native Location");
                if (pintoRemove != null)
                    map.Pins.Remove(pintoRemove);
                map.Pins.Add(new Pin() { Location = new Location(e.Latitude, e.Longitude), Label = "Native Location", Address = "" });
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
                Geolocation.StopListeningForeground();
                string status = "Stopped listening for foreground location updates";

                NativeLocationService.Instance.Stop();
                NativeLocationService.Instance.LocationChanged -= LocationService_LocationChanged;
                NativeLocationService.Instance.StatusChanged -= LocationService_StatusChanged;
            }
            catch (Exception ex)
            {
                // Unable to stop listening for location changes
                await Shell.Current.DisplayAlert("", ex.Message, "OK");
            }
        }
    }

}
