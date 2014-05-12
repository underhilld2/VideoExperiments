using System;
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
      StartRecoding = new RelayCommand(ExecuteStartRecoding, () => _canExecuteStartRecoding);
      StopRecoding = new RelayCommand(ExecuteStopRecoding, () => _canExecuteStopRecoding);

      ClickPreviewing = new RelayCommand(ExecuteClickPreviewing, () => _canExecutePreviewing);
      ClickRecording = new RelayCommand(ExecuteClickRecord, () => _canExecuteRecord);

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
    private bool _canExecutePreviewing = false;
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

    public RelayCommand StartRecoding { get; private set; }
    public RelayCommand StopRecoding { get; private set; }
    public RelayCommand StartPreview { get; private set; }
    public RelayCommand StopPreview { get; private set; }

    public RelayCommand ClickPreviewing { get; private set; }
    public RelayCommand ClickRecording { get; private set; }

    #endregion

    internal async void Initialze()
    {
      try
      {

        if (!IsInDesignMode)
        {
          await CaptureSource.InitializeAsync();

          if (CaptureSource.MediaCaptureSettings.VideoDeviceId != "" &&
              CaptureSource.MediaCaptureSettings.AudioDeviceId != "")
          {
            ControllerStatus = CameraControllerStatus.Ready;
            _canExecuteStartPreviewing = true;
            _canExecuteStartRecoding = true;
            StartPreview.RaiseCanExecuteChanged();
            StartRecoding.RaiseCanExecuteChanged();

            _canExecutePreviewing = true;
            ClickPreviewing.RaiseCanExecuteChanged();
            _canExecuteRecord = true;
            ClickRecording.RaiseCanExecuteChanged();
          }
          else
          {
            ControllerStatus = CameraControllerStatus.FailedToInitialize;
            _canExecuteStartPreviewing = false;
            _canExecuteStartRecoding = false;
            StartPreview.RaiseCanExecuteChanged();
            StartRecoding.RaiseCanExecuteChanged();

            _canExecutePreviewing = false;
            ClickPreviewing.RaiseCanExecuteChanged();
            _canExecuteRecord = false;
            ClickRecording.RaiseCanExecuteChanged();
          }
        }
        else
        {
          ControllerStatus = CameraControllerStatus.Ready;
          _canExecuteStartPreviewing = true;
          _canExecuteStartRecoding = true;
          StartPreview.RaiseCanExecuteChanged();
          StartRecoding.RaiseCanExecuteChanged();

          _canExecutePreviewing = true;
          ClickPreviewing.RaiseCanExecuteChanged();
          _canExecuteRecord = true;
          ClickRecording.RaiseCanExecuteChanged();
        }
      }
      catch (Exception)
      {
        ControllerStatus = CameraControllerStatus.FailedToInitialize;
        _canExecuteStartPreviewing = false;
        _canExecuteStartRecoding = false;
        StartPreview.RaiseCanExecuteChanged();
        StartRecoding.RaiseCanExecuteChanged();

        _canExecutePreviewing = false;
        ClickPreviewing.RaiseCanExecuteChanged();
        _canExecuteRecord = false;
        ClickRecording.RaiseCanExecuteChanged();
        throw;
      }
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
    }

    private async void ExecuteStopRecoding()
    {
      if (ControllerStatus == CameraControllerStatus.Recording)
      {
        await CaptureSource.StopRecordAsync();
      }
    }

    public void ExecuteClickRecord()
    {
      if (_isRecording == false)
      {
        _isRecording = true;
        ExecuteStartRecoding();
      }
      else
      {
        _isRecording = false;
        ExecuteStopRecoding();
      }
    }
    public async void ExecuteStartPreviewing()
    {
      try
      {

        _canExecuteStartPreviewing = false;
        StartPreview.RaiseCanExecuteChanged();
        await CaptureSource.StartPreviewAsync();
        _canExecuteStopPreviewing = true;
        StopPreview.RaiseCanExecuteChanged();


      }
      catch (Exception e)
      {
        _canExecuteStartPreviewing = false;
        _canExecuteStopPreviewing = false;
        StartPreview.RaiseCanExecuteChanged();
        StopPreview.RaiseCanExecuteChanged();
        throw;
      }

    }

    public async void ExecuteStopPreviewing()
    {
      try
      {
        _canExecuteStopPreviewing = false;
        StopPreview.RaiseCanExecuteChanged();
        await CaptureSource.StopPreviewAsync();
        _canExecuteStartPreviewing = true;
        StartPreview.RaiseCanExecuteChanged();


      }
      catch (Exception)
      {
        _canExecuteStartPreviewing = false;
        _canExecuteStopPreviewing = false;
        StartPreview.RaiseCanExecuteChanged();
        StopPreview.RaiseCanExecuteChanged();
        throw;
      }

    }

    public void ExecuteClickPreviewing()
    {
      if (_isPreviewing == false)
      {
        _isPreviewing = true;
        ExecuteStartPreviewing();
      }
      else
      {
        _isPreviewing = false;
        ExecuteStopPreviewing();
      }
    }


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