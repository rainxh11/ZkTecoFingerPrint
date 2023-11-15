using System;
using System.IO;
using SourceAFIS;

namespace ZkTecoFingerPrint;

public class ZkFingerPrintResult
{
    public ZkFingerPrintResult(byte[] bitmap, int width, int height, int dpi)
    {
        Bitmap = BitmapFormat.GetBitmap(bitmap, width, height).ToArray();
        File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now.ToFileTime()}.bmp"),bitmap);

        var image = new FingerprintImage(width, height, bitmap);
        Template = new FingerprintTemplate(image);
        TemplateHash = Extensions.Hash(Template.ToByteArray()).Replace("-", "");
    }

    public byte[] Bitmap { get; private set; }
    public FingerprintTemplate Template { get; }
    public string TemplateHash { get; }
}