using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MediaCaptureUniversal.CameraLogic
{
  public class CameraController : ViewModelBase, IDisposable
  {
    #region constructor

    public CameraController()
    {
      ControllerStatus = CameraControllerStatus.Uninitialized;
      CaptureSource = new MediaCapture();

      StartPreview = new RelayCommand(ExecuteStartPreviewing, () => _canExecuteStartPreviewing);
      StopPreview = new RelayCommand(ExecuteStopPreviewing, () => _canExecuteStopPreviewing);
      StartRecording = new RelayCommand(ExecuteStartRecoding, () => _canExecuteStartRecoding);
      StopRecording = new RelayCommand(ExecuteStopRecoding, () => _canExecuteStopRecoding);

      ClickPreview = new RelayCommand(ExecuteClickPreviewing, () => _canExecutePreview);
      ClickRecord = new RelayCommand(ExecuteClickRecord, () => _canExecuteRecord);

      CaptureSource.RecordLimitationExceeded += RecordLimitationExceeded;
      CaptureSource.Failed += Failed;

    }



    #endregion

    #region private variables
    private bool _disposed;
    private CameraControllerStatus _cameraControllerStatus;
    private bool _canExecuteStartPreviewing = false;
    private bool _canExecuteStartRecoding = false;
    private bool _canExecuteStopPreviewing = false;
    private bool _canExecuteStopRecoding = false;
    private bool _canExecutePreview = false;
    private bool _canExecuteRecord = false;
    private bool _isPreviewing = false;
    private bool _isRecording = false;

    private MediaCapture _captureSourceMgr;

    #endregion

    #region binding properties

    public MediaCapture CaptureSource
    {
      get { return _captureSourceMgr; }
      set
      {
        _captureSourceMgr = value;
        RaisePropertyChanged(() => CaptureSource);
      }
    }

    public CameraControllerStatus ControllerStatus
    {
      get { return _cameraControllerStatus; }
      private set
      {
        _cameraControllerStatus = value;
        RaisePropertyChanged(() => ControllerStatus);
      }
    }

    public bool IsPreviewing
    {
      get { return _isPreviewing; }
      set
      {
        _isPreviewing = value;
        RaisePropertyChanged(() => IsPreviewing);
      }
    }

    public bool IsRecording
    {
      get { return _isRecording; }
      set
      {
        _isRecording = value;
        RaisePropertyChanged(() => IsRecording);
      }
    }

    public RelayCommand StartRecording { get; private set; }
    public RelayCommand StopRecording { get; private set; }
    public RelayCommand StartPreview { get; private set; }
    public RelayCommand StopPreview { get; private set; }

    public RelayCommand ClickPreview { get; private set; }
    public RelayCommand ClickRecord { get; private set; }

    #endregion

    internal async Task<int> Initialze()
    {
      int returnVar = 0;
      try
      {

        if (!IsInDesignMode)
        {
          await CaptureSource.InitializeAsync();

          if (CaptureSource.MediaCaptureSettings.VideoDeviceId != "" &&
              CaptureSource.MediaCaptureSettings.AudioDeviceId != "")
          {
            ControllerStatus = CameraControllerStatus.Ready;
            SetCanExecuteStartRecording(true);
            SetCanExecuteStartPreview(true);

            SetCanExecuteClickPreview(true);
            SetCanExecuteClickRecord(true);
            IsRecording = false;
            IsPreviewing = false;
            returnVar = 1;
          }
          else
          {
            ControllerStatus = CameraControllerStatus.FailedToInitialize;
            SetCanExecuteStartRecording(false);
            SetCanExecuteStartPreview(false);

            SetCanExecuteClickPreview(false);
            SetCanExecuteClickRecord(false);
          }
        }
        else
        {
          ControllerStatus = CameraControllerStatus.Ready;
          SetCanExecuteStartRecording(true);
          SetCanExecuteStartPreview(true);

          SetCanExecuteClickPreview(true);
          SetCanExecuteClickRecord(true);

          IsRecording = false;
          IsPreviewing = false;
          returnVar = 1;
        }
      }
      catch (Exception)
      {
        ControllerStatus = CameraControllerStatus.FailedToInitialize;


        SetCanExecuteStartRecording(false);
        SetCanExecuteStartPreview(false);

        SetCanExecuteClickPreview(false);
        SetCanExecuteClickRecord(false);
        throw;
      }
      return returnVar;
    }

    public async void Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
    {
      ControllerStatus = CameraControllerStatus.CaptureFailure;
      try
      {
      }
      catch (Exception e)
      {
      }
    }

    public async void RecordLimitationExceeded(MediaCapture currentCaptureObject)
    {
      ControllerStatus = CameraControllerStatus.RecordLimitationExceeded;
    }


    #region Methods
    private void ExecuteStartRecoding()
    {

      if (ControllerStatus == CameraControllerStatus.Ready)
      {
        if (IsRecording == false)
        {
          IsRecording = true;
          SetCanExecuteStartRecording(false);
          SetCanExecuteStopRecording(true);

        }
      }


    }

    private async void ExecuteStopRecoding()
    {
      if (IsRecording == true)
      {
        //await CaptureSource.StopRecordAsync();
        IsRecording = false;
        SetCanExecuteStopRecording(false);
        SetCanExecuteStartRecording(true);


      }
    }

    public void ExecuteClickRecord()
    {
      if (IsRecording == false)
      {
        ExecuteStartRecoding();
      }
      else
      {
        ExecuteStopRecoding();
      }
    }
    public async void ExecuteStartPreviewing()
    {
      try
      {

        SetCanExecuteStartPreview(false);
        await CaptureSource.StartPreviewAsync();
        SetCanExecuteStopPreview(true);
        IsPreviewing = true;

      }
      catch (Exception e)
      {
        SetCanExecuteStartPreview(false);
        SetCanExecuteStopPreview(false);
        throw;
      }

    }

    public async void ExecuteStopPreviewing()
    {
      try
      {
        SetCanExecuteStopPreview(false);
        await CaptureSource.StopPreviewAsync();
        SetCanExecuteStartPreview(true);
        IsPreviewing = false;

      }
      catch (Exception)
      {
        SetCanExecuteStartPreview(false);
        SetCanExecuteStopPreview(false);
        throw;
      }

    }

    public void ExecuteClickPreviewing()
    {
      if (IsPreviewing == false)
      {
        ExecuteStartPreviewing();
      }
      else
      {
        ExecuteStopPreviewing();
      }
    }


    #region helpers
    private void SetCanExecuteStartPreview(bool flag)
    {
      _canExecuteStartPreviewing = flag;
      StartPreview.RaiseCanExecuteChanged();
    }

    private void SetCanExecuteStopPreview(bool flag)
    {
      _canExecuteStopPreviewing = flag;
      StopPreview.RaiseCanExecuteChanged();
    }

    private void SetCanExecuteStartRecording(bool flag)
    {
      _canExecuteStartRecoding = flag;
      StartRecording.RaiseCanExecuteChanged();
    }

    private void SetCanExecuteStopRecording(bool flag)
    {
      _canExecuteStopRecoding = flag;
      StopRecording.RaiseCanExecuteChanged();
    }

    private void SetCanExecuteClickPreview(bool flag)
    {
      _canExecutePreview = flag;
      ClickPreview.RaiseCanExecuteChanged();
    }
    private void SetCanExecuteClickRecord(bool flag)
    {
      _canExecuteRecord = flag;
      ClickRecord.RaiseCanExecuteChanged();
    }
    #endregion

    #endregion

    #region IDisposable Interface

    // Public implementation of Dispose pattern callable by consumers. 
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern. 
    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      if (disposing)
      {
        _captureSourceMgr.Dispose();
      }

      // Free any unmanaged objects here. 
      //
      _disposed = true;
    }

    #endregion
  }

}