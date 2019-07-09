using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace MetaClip
{
    class MetaClipBootstrapper :  UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
        }

        protected override void InitializeShell()
        {
            // Register views in the container
            UnityContainer container = (UnityContainer)this.Container;
            container.RegisterType<object, Views.MainPanel>("MainPanel");
            container.RegisterType<object, Views.LibrarySettings>("LibrarySettings");
            container.RegisterType<object, Views.AddClip>("AddClip");
            container.RegisterType<object, Views.CategoryManager>("CategoryManager");

            SetDefaultView();

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Title = "MetaClip";
            Application.Current.MainWindow.Show();            
        }

        public void SetDefaultView()
        {            
            DependencyObject view = Shell;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "MainPanel");
        }
    }

}
