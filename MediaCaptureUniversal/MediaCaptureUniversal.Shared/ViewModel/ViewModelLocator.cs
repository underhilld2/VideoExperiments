using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Ioc;
using MediaCaptureUniversal.CameraLogic;
using Microsoft.Practices.ServiceLocation;

namespace MediaCaptureUniversal.ViewModel
{
  public class ViewModelLocator
  {
    public CameraController CameraController
    {
      get
      {
        return ServiceLocator.Current.GetInstance<CameraController>();
      }
    }

    static ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
      SimpleIoc.Default.Register<CameraController>();
    }
  }
}
