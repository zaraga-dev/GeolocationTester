using Android.Content;
using Android.Locations;
using Android.Net.Wifi.Rtt;
using Android.OS;
using zaraga.plugins.NativeLocation.Models;
using AndroidApp = Android.App.Application;
using Location = Android.Locations.Location;

namespace zaraga.plugins.NativeLocation;

public partial class NativeLocationService : Java.Lang.Object, ILocationListener
{
    /// <summary>
    /// minimum time interval between location updates in milliseconds
    /// </summary>
    private readonly long MIN_TIME_MS = 500;

    /// <summary>
    /// minimum distance between location updates in meters
    /// </summary>
    private readonly float MIN_DISTANCE_MTS = 0.5f;

    private LocationManager _androidLocationManager;

    protected void AndroidStop()
    {
        OnStatusChanged($"LocationService->Stop");
        _androidLocationManager?.RemoveUpdates(this);
    }

    protected void AndroidInitialize()
    {
        OnStatusChanged($"LocationService->Initialize");
        _androidLocationManager ??= (LocationManager)AndroidApp.Context.GetSystemService(Context.LocationService);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                OnStatusChanged("Permission for location is not granted, we can't get location updates");
                return;
            }

            if (!_androidLocationManager.IsLocationEnabled)
            {
                OnStatusChanged("Location is not enabled, we can't get location updates");
                return;
            }

            if (!_androidLocationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                OnStatusChanged("GPS Provider is not enabled, we can't get location updates");
                return;
            }

            _androidLocationManager.RequestLocationUpdates(LocationManager.GpsProvider, MIN_TIME_MS, MIN_DISTANCE_MTS, this);
        });
    }

    public void OnLocationChanged(Location location)
    {
        if (location != null)
        {
            float verticalMeters = 0;
            bool mockProvider = false;

            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                verticalMeters = location.VerticalAccuracyMeters;
            }

            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                mockProvider = location.Mock;
            }
            else
            {
                mockProvider = location.IsFromMockProvider;
            }

            OnLocationChanged(new NativeLocationModel(
                location.Latitude
                , location.Longitude
                , location.Accuracy
                , location.Altitude
                , location.Speed
                , DateTime.Now
                , verticalMeters
                , reducedAccuracy: false
                , course: 0
                , mockProvider));
        }
    }

    public void OnProviderDisabled(string provider)
    {
        //inform your services that we stop getting updates
        OnStatusChanged($"{provider} has been disabled");
    }

    public void OnProviderEnabled(string provider)
    {
        //inform your services that we start getting updates
        OnStatusChanged($"{provider} now enabled");
    }

    public void OnStatusChanged(string provider, Availability status, Bundle extras)
    {
        //inform your services that provides status has been changed
        OnStatusChanged($"{provider} change his status and now it's {status}");
    }
}
