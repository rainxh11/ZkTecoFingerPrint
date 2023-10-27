#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ZkTecoFingerPrint;

public class ZkFingerPrintDevice : IDisposable
{
    internal ZkFingerPrintDevice(IntPtr handle, int width, int height, int dpi,
                                 string serialNumber)
    {
        Handle = handle;
        Width = width;
        Height = height;
        Dpi = dpi;
        SerialNumber = serialNumber;
        ZkTecoFingerHost.OnClosing += OnClosing;
    }

    public IntPtr Handle { get; }
    public int Width { get; }
    public int Height { get; }
    public int Dpi { get; }
    public string SerialNumber { get; private set; }

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

    public async Task<ZkResult<ZkFingerPrintResult?>> AcquireFingerprint(byte[] buffer, CancellationToken ct = default)
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

        return new ZkResult<ZkFingerPrintResult?>(response, response is ZkResponse.Ok
                                                                ? new ZkFingerPrintResult(buffer, Width, Height, Dpi)
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