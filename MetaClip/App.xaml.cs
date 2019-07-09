using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using MetaClip.ViewModels;

namespace MetaClip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RunInReleaseMode();
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static void RunInReleaseMode()
        {            
            try
            {
                MetaClipBootstrapper bootstrapper = new MetaClipBootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
