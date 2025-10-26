using CoreLocation;
using Microsoft.Maui.Platform;

namespace zaraga.plugins.NativeLocation;

public partial class NativeLocationService
{
    private CLLocationManager _iosLocationManager;

    protected void IosStop()
    {
        OnStatusChanged($"LocationService->Stop");
        if (_iosLocationManager is null)
            return;

        _iosLocationManager.StopUpdatingLocation();
        _iosLocationManager.LocationsUpdated -= LocationsUpdated;
    }

    protected void IosInitialize()
    {
        OnStatusChanged($"LocationService->Initialize");
        _iosLocationManager ??= new CLLocationManager()
        {
            DesiredAccuracy = CLLocation.AccuracyBest,
            DistanceFilter = CLLocationDistance.FilterNone,
            PausesLocationUpdatesAutomatically = false
        };

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                OnStatusChanged("Permission for location is not granted, we can't get location updates");
                return;
            }

            _iosLocationManager.RequestAlwaysAuthorization();
            _iosLocationManager.LocationsUpdated += LocationsUpdated;
            _iosLocationManager.StartUpdatingLocation();
        });
    }

    private void LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
    {
        var locations = e.Locations;
        double accuracy = 0;
        if (OperatingSystem.IsIOSVersionAtLeast(14) || OperatingSystem.IsMacCatalystVersionAtLeast(14))
        {
            //accuracy = locations[^1].HorizontalAccuracy;
            accuracy = locations[^1].CourseAccuracy;
        }

        LocationChanged?.Invoke(this, new Microsoft.Maui.Devices.Sensors.Location()
        {
            Latitude = locations[^1].Coordinate.Latitude,
            Longitude = locations[^1].Coordinate.Longitude,
            Accuracy = accuracy,
            Altitude = locations[^1].Altitude,
            Speed = locations[^1].Speed,
            Timestamp = locations[^1].Timestamp.ToDateTime(),
            VerticalAccuracy = locations[^1].VerticalAccuracy,
            ReducedAccuracy = false,
            Course = (float)locations[^1].Course
        });
    }
}
