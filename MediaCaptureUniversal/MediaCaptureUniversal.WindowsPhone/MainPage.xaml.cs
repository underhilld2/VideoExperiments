using System;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using MediaCaptureUniversal.CameraLogic;


namespace MediaCaptureUniversal
{
  /// <summary>
  ///   An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private readonly String PHOTO_FILE_NAME = "photo.jpg";
    private readonly String VIDEO_FILE_NAME = "video.mp4";
    private MediaCapture _mMediaCaptureMgr;
    private bool _mBPreviewing;
    private bool _mBRecording;
    private bool m_bSuspended;

    private TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs>
      m_mediaPropertyChanged;

    private StorageFile m_photoStorageFile;
    private StorageFile m_recordStorageFile;
    private CameraController _cameraController;

    public MainPage()
    {
      InitializeComponent();

      NavigationCacheMode = NavigationCacheMode.Required;
      m_mediaPropertyChanged += SystemMediaControls_PropertyChanged;
      Unloaded += OnUnloaded;

    }

    private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
    {
      throw new NotImplementedException();
    }


    /// <summary>
    ///   Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">
    ///   Event data that describes how this page was reached.
    ///   This parameter is typically used to configure the page.
    /// </param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      // TODO: Prepare page for display here.

      // TODO: If your application contains multiple pages, ensure that you are
      // handling the hardware Back button by registering for the
      // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
      // If you are using the NavigationHelper provided by some templates,
      // this event is handled for you.
    }

    private void ScenarioInit()
    {
      //btnStartDevice1.IsEnabled = true;
      //btnStartPreview1.IsEnabled = false;
      //btnStartStopRecord1.IsEnabled = false;
      //btnStartStopRecord1.Content = "StartRecord";
      //btnTakePhoto1.IsEnabled = false;
      //btnTakePhoto1.Content = "TakePhoto";

      _mBRecording = false;
      _mBPreviewing = false;
      m_bSuspended = false;

      previewElement1.Source = null;
      //playbackElement1.Source = null;
      //imageElement1.Source = null;
      //sldBrightness.IsEnabled = false;
      //sldContrast.IsEnabled = false;

      //ShowStatusMessage("");
    }

    private async void ScenarioClose()
    {
      try
      {
        if (_mBRecording)
        {
          //ShowStatusMessage("Stopping Record");

          await _mMediaCaptureMgr.StopRecordAsync();
          _mBRecording = false;
        }
        if (_mBPreviewing)
        {
          //ShowStatusMessage("Stopping preview");
          await _mMediaCaptureMgr.StopPreviewAsync();
          _mBPreviewing = false;
        }

        if (_mMediaCaptureMgr != null)
        {
          //ShowStatusMessage("Stopping Camera");
          previewElement1.Source = null;
          _mMediaCaptureMgr.Dispose();
        }
      }
      catch (Exception e)
      {
        //ShowExceptionMessage(e);
      }
    }

    private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender,
      SystemMediaTransportControlsPropertyChangedEventArgs e)
    {
      switch (e.Property)
      {
        case SystemMediaTransportControlsProperty.SoundLevel:
          await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
          {
            if (sender.SoundLevel != SoundLevel.Muted)
            {
              ScenarioInit();
            }
            else
            {
              ScenarioClose();
            }
          });
          break;

        default:
          break;
      }
    }

    public async void RecordLimitationExceeded(MediaCapture currentCaptureObject)
    {
      if (_mBRecording)
      {
        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        {
          try
          {
            //ShowStatusMessage("Stopping Record on exceeding max record duration");
            await _mMediaCaptureMgr.StopRecordAsync();
            _mBRecording = false;
            //btnStartStopRecord1.Content = "StartRecord";
            //btnStartStopRecord1.IsEnabled = true;
            //ShowStatusMessage("Stopped record on exceeding max record duration:" + m_recordStorageFile.Path);

            if (!_mMediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
            {
              //if camera does not support record and Takephoto at the same time
              //enable TakePhoto button again, after record finished
              //btnTakePhoto1.Content = "TakePhoto";
              //btnTakePhoto1.IsEnabled = true;
            }
          }
          catch (Exception e)
          {
            //ShowExceptionMessage(e);
          }
        });
      }
    }

    public async void Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
    {
      try
      {
        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          //ShowStatusMessage("Fatal error" + currentFailure.Message);
        });
      }
      catch (Exception e)
      {
        //ShowExceptionMessage(e);
      }
    }

    internal async void btnStartDevice_Click(Object sender, RoutedEventArgs e)
    {
      try
      {
        //btnStartDevice1.IsEnabled = false;
        //ShowStatusMessage("Starting device");
        _mMediaCaptureMgr = new MediaCapture();
        await _mMediaCaptureMgr.InitializeAsync();

        if (_mMediaCaptureMgr.MediaCaptureSettings.VideoDeviceId != "" &&
            _mMediaCaptureMgr.MediaCaptureSettings.AudioDeviceId != "")
        {
          //btnStartPreview1.IsEnabled = true;
          //btnStartStopRecord1.IsEnabled = true;
          //btnTakePhoto1.IsEnabled = true;

          //ShowStatusMessage("Device initialized successful");

          _mMediaCaptureMgr.RecordLimitationExceeded += RecordLimitationExceeded;
          _mMediaCaptureMgr.Failed += Failed;
        }
      }
      catch (Exception exception)
      {
        //ShowExceptionMessage(exception);
      }
    }

    internal async void btnStartPreview_Click(Object sender, RoutedEventArgs e)
    {
      _mBPreviewing = false;
      try
      {
        //ShowStatusMessage("Starting preview");
        //btnStartPreview1.IsEnabled = false;

        //previewCanvas1.Visibility = Windows.UI.Xaml.Visibility.Visible;
        previewElement1.Source = _mMediaCaptureMgr;
        await _mMediaCaptureMgr.StartPreviewAsync();
        //if ((m_mediaCaptureMgr.VideoDeviceController.Brightness != null) && m_mediaCaptureMgr.VideoDeviceController.Brightness.Capabilities.Supported)
        //{
        //  SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Brightness, sldBrightness);
        //}
        //if ((m_mediaCaptureMgr.VideoDeviceController.Contrast != null) && m_mediaCaptureMgr.VideoDeviceController.Contrast.Capabilities.Supported)
        //{
        //  SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Contrast, sldContrast);
        //}
        _mBPreviewing = true;
        //ShowStatusMessage("Start preview successful");
      }
      catch (Exception exception)
      {
        _mBPreviewing = false;
        previewElement1.Source = null;
        //btnStartPreview1.IsEnabled = true;
        //ShowExceptionMessage(exception);
      }
    }

    private void btnStartDevice2_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnStartPreview2_Click(object sender, RoutedEventArgs e)
    {
      var temp = this.DataContext as CameraController;

      temp.ExecuteStartPreviewing();
    }

    private async void OnLoad(object sender, RoutedEventArgs e)
    {
      var temp = this.DataContext as CameraController;
      await temp.Initialze();
      PreviewElement1ACaptureElement.Source = temp.CaptureSource;
    }

  }
}