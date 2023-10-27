using SourceAFIS;

namespace ZkTecoFingerPrint;

public class ZkFingerPrintResult
{
    public ZkFingerPrintResult(byte[] bitmap, int width, int height, int dpi)
    {
        Bitmap = bitmap;
        var image = new FingerprintImage(width, height, bitmap, new FingerprintImageOptions
                                                                {
                                                                    Dpi = dpi
                                                                });
        Template = new FingerprintTemplate(image);
        TemplateHash = Extensions.Hash(Template.ToByteArray()).Replace("-", "");
    }

    public byte[] Bitmap { get; private set; }
    public FingerprintTemplate Template { get; }
    public string TemplateHash { get; }
}