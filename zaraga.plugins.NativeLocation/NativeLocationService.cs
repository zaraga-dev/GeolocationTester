using zaraga.plugins.NativeLocation.Models;

namespace zaraga.plugins.NativeLocation
{
    public partial class NativeLocationService
    {
        public event EventHandler<Microsoft.Maui.Devices.Sensors.Location> LocationChanged;
        public event EventHandler<string> StatusChanged;

        public void Initialize()
        {
#if ANDROID
            AndroidInitialize();
#elif IOS
        IosInitialize();
#endif
        }

        public void Stop()
        {
#if ANDROID
            AndroidStop();
#elif IOS
        IosStop();
#endif
        }

        protected virtual void OnLocationChanged(NativeLocationModel e)
        {
            LocationChanged?.Invoke(this, new Microsoft.Maui.Devices.Sensors.Location()
            {
                Accuracy = e.Accuracy,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                Altitude = e.Altitude,
                Speed = e.Speed,
                Timestamp = e.Timestamp,
                VerticalAccuracy = e.VerticalAccuracy,
                ReducedAccuracy = e.ReducedAccuracy,
                Course = e.Course,
                IsFromMockProvider = e.IsFromMockProvider
            });
        }

        protected virtual void OnStatusChanged(string e)
        {
            StatusChanged?.Invoke(this, e);
        }
    }
}
