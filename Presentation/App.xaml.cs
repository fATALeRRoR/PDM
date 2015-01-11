using System;
using System.Globalization;
using System.Runtime;
using System.Windows;
using System.Windows.Markup;
using PDM.Client.Common;
using PDM.Client.Exceptions;
using PDM.Client.Helpers;
using PDM.Client.Settings;

namespace PDM.Client
{
    public partial class App
    {
        public App()
        {
            //Improve startup performance
            //http://blogs.msdn.com/b/dotnet/archive/2012/10/18/an-easy-solution-for-improving-app-launch-performance.aspx
            ProfileOptimization.SetProfileRoot(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            ProfileOptimization.StartProfile("PDM.Startup.Profile");

            Logging.Debug("Application started");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //Set culture for WPF
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof (FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exc = e.ExceptionObject as Exception;

            if (exc is BusinessException|| exc.InnerException is BusinessException)
            {
                new MessageBoxDialog(Constants.Error, e.ExceptionObject.ToString()).Show();                
            }
            else
            {
                Logging.Debug("Technical error");
                Logging.Error(e.ExceptionObject);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ApplicationSettings.SaveSerializedSettings();

            base.OnExit(e);
        }
    }
}