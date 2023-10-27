#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ZkTecoFingerPrint;

public class ZkTecoFingerHost
{
    public static event Action? OnClosing;

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_Init();

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_Terminate();

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetDeviceCount();

    [DllImport("libzkfp.dll")]
    private static extern IntPtr ZKFPM_OpenDevice(int index);


    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetCaptureParamsEx(
        IntPtr handle,
        ref int width,
        ref int height,
        ref int dpi);

    [DllImport("libzkfp.dll")]
    private static extern int ZKFPM_GetParameters(
        IntPtr handle,
        int nParamCode,
        IntPtr paramValue,
        ref int cbParamValue);


    public ZkResult<int> Initialize()
    {
        var result = ZKFPM_Init();
        return new ZkResult<int>((ZkResponse)result, result);
    }

    public ZkResponse Close()
    {
        OnClosing?.Invoke();
        return (ZkResponse)ZKFPM_Terminate();
    }

    public int GetDeviceCount()
    {
        return ZKFPM_GetDeviceCount();
    }

    private static ZkResponse GetParameters(IntPtr devHandle, int code, byte[] paramValue, ref int size)
    {
        if (devHandle == IntPtr.Zero)
            return ZkResponse.InvalidHandle;
        size = paramValue.Length;
        var num = Marshal.AllocHGlobal(size);
        var parameters = (ZkResponse)ZKFPM_GetParameters(devHandle, code, num, ref size);
        if (parameters == ZkResponse.Ok)
            Marshal.Copy(num, paramValue, 0, size);
        Marshal.FreeHGlobal(num);
        return parameters;
    }

    public ZkDeviceResult OpenDevice(int index)
    {
        var handle = ZKFPM_OpenDevice(index);
        if (handle == IntPtr.Zero)
            return new ZkDeviceResult(ZkResponse.InvalidHandle);

        var numArray = new byte[64];
        var size = 64;
        GetParameters(handle, 1103, numArray, ref size);
        var serialNumber = Encoding.Default.GetString(numArray);
        int width = 0, height = 0, dpi = 0;
        var response = (ZkResponse)ZKFPM_GetCaptureParamsEx(handle, ref width, ref height, ref dpi);
        if (response is not ZkResponse.Ok)
            return new ZkDeviceResult(response);
        var device = new ZkFingerPrintDevice(handle, width, height, dpi, serialNumber);
        return new ZkDeviceResult(response, device);
    }
}