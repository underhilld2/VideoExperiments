namespace MediaCaptureUniversal.CameraLogic
{
  public enum CameraControllerStatus
  {
    RecordLimitationExceeded = -3,
    CaptureFailure = -2,
    FailedToInitialize = -1,
    Uninitialized = 0,
    Initialized = 1,
    Ready = 2,
    Recording = 3,
  }
}
