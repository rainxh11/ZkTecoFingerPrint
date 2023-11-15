#nullable enable
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZkTecoFingerPrint;

public class ZkFingerDeviceInfo
{
    public IntPtr Handle { get; set; }
    public string Name { get; set; }
    public string SerialNumber { get; set; }
}

public class ZkFingerPrintDevice : IDisposable
{
    internal ZkFingerPrintDevice(IntPtr handle, int width, int height, int dpi,
                                 string serialNumber, string name)
    {
        Handle = handle;
        Width = width;
        Height = height;
        Dpi = dpi;
        SerialNumber = serialNumber;
        Name = name;
        ZkTecoFingerHost.OnClosing += OnClosing;
    }

    public IntPtr Handle { get; }
    public int Width { get; }
    public int Height { get; }
    public int Dpi { get; }
    public string Name { get; }
    public string SerialNumber { get; private set; }
    public override string ToString()
    {
        return $"{Name}, SN: {SerialNumber}";
    }

    public void Dispose()
    {
        try
        {
            ZkTecoFingerHost.OnClosing -= OnClosing;
            ZKFPM_CloseDevice(Handle);
        }
        catch
        {
        }
    }


    public async Task<ZkResult<ZkFingerPrintResult?>> AcquireFingerprintAsync(CancellationToken ct = default)
        {
            var imageDataSizeBuffer = ArrayPool<byte>.Shared.Rent(32);
            var dataSize = 32;
            var imageDataSizeResult = ZkTecoFingerHost.GetParameters(Handle, 106, imageDataSizeBuffer, ref dataSize);
            if (imageDataSizeResult != ZkResponse.Ok)
            {
                ArrayPool<byte>.Shared.Return(imageDataSizeBuffer);
                return new ZkResult<ZkFingerPrintResult?>(imageDataSizeResult, null);
            }
            var imageDataSize = BitConverter.ToInt32(imageDataSizeBuffer, 0);

            var buffer = new byte[imageDataSize];
            var pointer = Marshal.AllocHGlobal(buffer.Length);
            var template = new byte[imageDataSize];
            var templatePointer = Marshal.AllocHGlobal(template.Length);
            var size = template.Length;
            var response =
                await
                    Task.Run(() => (ZkResponse)ZKFPM_AcquireFingerprint(Handle, pointer, (uint)buffer.Length, templatePointer, ref size),
                             ct);
            if (response == ZkResponse.Ok)
                Marshal.Copy(pointer, buffer, 0, buffer.Length);
            Marshal.FreeHGlobal(pointer);
            Marshal.FreeHGlobal(templatePointer);

        ArrayPool<byte>.Shared.Return(imageDataSizeBuffer);
            //var bitmap = ZkTecoFingerPrint.BitmapFormat.GetBitmap(buffer, Width, Height).ToArray();

            return new ZkResult<ZkFingerPrintResult?>(response, response is ZkResponse.Ok
                                                                    ? new ZkFingerPrintResult(buffer, Width, Height, new ZkFingerDeviceInfo()
                                                                        {
                                                                            Handle = Handle,
                                                                            Name = Name,
                                                                            SerialNumber = SerialNumber
                                                                        })
                                                                    : null);
        }

        public async Task<ZkResult<ZkFingerPrintResult?>> AcquireFingerprintAsync(byte[] buffer, CancellationToken ct = default)
    {
        var pointer = Marshal.AllocHGlobal(buffer.Length);
        var template = new byte[2048];
        var templatePointer = Marshal.AllocHGlobal(template.Length);
        var size = template.Length;
        var response =
            await
                Task.Run(() => (ZkResponse)ZKFPM_AcquireFingerprint(Handle, pointer, (uint)buffer.Length, templatePointer, ref size),
                         ct);
        if (response == ZkResponse.Ok)
            Marshal.Copy(pointer, buffer, 0, buffer.Length);
        Marshal.FreeHGlobal(pointer);
        Marshal.FreeHGlobal(templatePointer);

        return new ZkResult<ZkFingerPrintResult?>(response, response is ZkResponse.Ok
                                                                ? new ZkFingerPrintResult(buffer, Width, Height, new ZkFingerDeviceInfo()
                                                                    {
                                                                        Handle = Handle,
                                                                        Name = Name,
                                                                        SerialNumber = SerialNumber
                                                                    })
                                                                : null);
    }

    private void OnClosing()
    {
        Dispose();
    }

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_CloseDevice(IntPtr handle);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_AcquireFingerprint(
        IntPtr handle,
        IntPtr fpImage,
        uint cbFpImage,
        IntPtr fpTemplate,
        ref int cbTemplate);

}