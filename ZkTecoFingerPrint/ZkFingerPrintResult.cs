#nullable enable
using SourceAFIS;

namespace ZkTecoFingerPrint;


public class ZkFingerPrintResult 
{
    public ZkFingerPrintResult(byte[] bitmap, int width, int height, ZkFingerDeviceInfo deviceInfo = null)
    {
        Bitmap = BitmapFormat.GetBitmap(bitmap, width, height).ToArray();
        var image = new FingerprintImage(width, height, bitmap);
        Template = new FingerprintTemplate(image);
        TemplateHash = Extensions.Hash(Template.ToByteArray()).Replace("-", "");
        DeviceInfo = deviceInfo;
    }

    public byte[] Bitmap { get; private set; }
    public FingerprintTemplate Template { get; }
    public string TemplateHash { get; }
    public ZkFingerDeviceInfo? DeviceInfo { get; private set; }
}