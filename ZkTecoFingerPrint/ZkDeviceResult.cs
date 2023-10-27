#nullable enable
using System;

namespace ZkTecoFingerPrint;

public class ZkDeviceResult : ZkResult<ZkFingerPrintDevice?>, IDisposable
{
    internal ZkDeviceResult(ZkResponse response, ZkFingerPrintDevice? value = null)
        : base(response, value)
    {
    }

    public override bool IsSuccess => Response == ZkResponse.Ok && Value != null;

    public void Dispose()
    {
        Value?.Dispose();
    }
}