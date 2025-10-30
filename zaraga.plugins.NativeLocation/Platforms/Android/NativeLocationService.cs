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
    private static readonly long MIN_TIME_MS = 1000;
    private static readonly float MIN_DISTANCE_M = 0.5f;

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
                OnStatusChanged("Permission for location is not granted, can't get location updates");
                return;
            }

            if (!_androidLocationManager.IsLocationEnabled)
            {
                OnStatusChanged("Location is not enabled, can't get location updates");
                return;
            }

            if (!_androidLocationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                OnStatusChanged("GPS Provider is not enabled, can't get location updates");
                return;
            }
            _androidLocationManager.RequestLocationUpdates(LocationManager.GpsProvider, MIN_TIME_MS, MIN_DISTANCE_M, this);
        });
    }

    public void OnLocationChanged(Location location)
    {
        if (location != null)
        {
            OnLocationChanged(new NativeLocationModel(location.Latitude, location.Longitude, location.Accuracy));
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