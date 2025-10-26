using zaraga.plugins.NativeLocation.Models;

namespace zaraga.plugins.NativeLocation;

public partial class NativeLocationService
{
    private static NativeLocationService _instance;

    public static NativeLocationService Instance => _instance ??= new NativeLocationService();
    public event EventHandler<NativeLocationModel> LocationChanged;
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
        LocationChanged?.Invoke(this, e);
    }

    protected virtual void OnStatusChanged(string e)
    {
        StatusChanged?.Invoke(this, e);
    }
}