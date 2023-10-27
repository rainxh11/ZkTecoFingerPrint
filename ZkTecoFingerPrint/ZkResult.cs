namespace ZkTecoFingerPrint;

public class ZkResult<T>
{
    internal ZkResult(ZkResponse response, T value)
    {
        Value = value;
        Response = response;
    }

    public ZkResponse Response { get; }
    public virtual bool IsSuccess => Response == ZkResponse.Ok;
    public T Value { get; }
}