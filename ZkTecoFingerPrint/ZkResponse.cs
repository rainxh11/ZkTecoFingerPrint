namespace ZkTecoFingerPrint
{
  public enum ZkResponse
  {
    AlreadyInit = 1,
    Ok = 0,
    InitLibrary = -1,
    Init = -2,
    NoDevice = -3,
    NotSupported = -4,
    InvalidParameter = -5,
    Open = -6,
    InvalidHandle = -7,
    Capture = -8,
    ExtractFingerPrint = -9,
    Abort = -10,
    NotEnoughMemory = -11,
    Busy = -12,
    AddFinger = -13,
    DeleteFinger = -14,
    Fail = -17,
    Cancel = -18,
    VerifyFingerPrint = -20,
    Merge = -22,
    NotOpened = -23,
    NotInit = -24,
    AlreadyOpened = -25,
    LoadImage = -26,
    AnalyzeImage = -27,
    Timeout = -28,
  }
}
