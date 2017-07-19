/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TekkenEditor"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using TekkenEditor.Helper;

namespace TekkenEditor.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SetupNavigation();
            SimpleIoc.Default.Register<IFileService, FileService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SlotPageViewModel>();
            SimpleIoc.Default.Register<EditPageViewModel>();
        }
        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("DefaultPage", new Uri("../Views/DefaultPage.xaml", UriKind.Relative));
            navigationService.Configure("EditPage", new Uri("../Views/EditPage.xaml", UriKind.Relative));
            navigationService.Configure("SlotPage", new Uri("../Views/SlotPage.xaml", UriKind.Relative));

            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SlotPageViewModel slotPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SlotPageViewModel>();
            }
        }

        public EditPageViewModel editPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditPageViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}