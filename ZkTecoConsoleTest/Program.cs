using ZkTecoFingerPrint;


ZkTecoFingerHost.Initialize();
var deviceCount = ZkTecoFingerHost.GetDeviceCount();
using var device = ZkTecoFingerHost.OpenDevice(0);
if (device.IsSuccess)
{
    while (true)
    {
        var fingerprint = await device.Value?.AcquireFingerprintAsync();
        if (fingerprint.IsSuccess)
        {
            var image = fingerprint.Value?.Bitmap;
        }
    }

}


Console.ReadKey();
