using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Forms;

namespace MetaClip.ViewModels
{
    class LibrarySettings : ViewModelBase
    {
        public LibrarySettings()
        {
            this.CloseCommand = new DelegateCommand<object>(this.OnClose, this.CanClose);
            this.ChangeCommand = new DelegateCommand<object>(this.OnChange, this.CanChange);
            this.ExportCommand = new DelegateCommand<object>(this.OnExport, this.CanExport);
            this.LibraryPath = MetaclipApplication.Instance().LibraryPath;           
        }

        private string _libraryPath = "";
        public string LibraryPath
        {
            get { return _libraryPath; }
            set { _libraryPath = value; }
        }

        public ICommand CloseCommand { get; private set; }
        private bool CanClose(object arg) { return true; }
        private void OnClose(object arg)
        {
            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "MainPanel");
        }

        public ICommand ChangeCommand { get; private set; }
        private bool CanChange(object arg) { return true; }
        private void OnChange(object arg)
        {
            MetaclipApplication.Instance().State.ChangeLibrary();
        }

        public ICommand ExportCommand { get; private set; }
        private bool CanExport(object arg) { return true; }
        private void OnExport(object arg)
        {
            MetaclipApplication.Instance().State.ExportLibrary();
        }

    
    
    
    }
}
