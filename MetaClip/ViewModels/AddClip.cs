using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;


namespace MetaClip.ViewModels
{
     class AddClip : ViewModelBase
    {         
        public AddClip()
        {        
            this.CloseCommand = new DelegateCommand<object>(this.OnClose, this.CanClose);            
            this.AddCommand = new DelegateCommand<object>(this.OnAdd, this.CanAdd);
            this.NewCommand = new DelegateCommand<object>(this.OnNew, this.CanNew);
            var lib = new MetaClip.Models.ClipLibrary(MetaclipApplication.Instance().LibraryPath);
            this.CategoryList = lib.Categories;            
        }

        public ICommand CloseCommand { get; private set; }
        private bool CanClose(object arg) { return true; }
        private void OnClose(object arg)
        {
            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "MainPanel");
            this.ClipName = ""; 
        }

        public ICommand AddCommand { get; private set; }
        private bool CanAdd(object arg) { return true; }     
        private void OnAdd(object arg)
        {
            if (Models.ClipLibrary.ValidateName(this.ClipName, MetaclipApplication.Instance().LibraryPath))
            {
                ClipMgrLibrary.ClipManager ClipMgr = new ClipMgrLibrary.ClipManager();
                ClipMgrLibrary.Clip Clip = ClipMgr.MakeClip(this.ClipName);
                string path;

                if (this.SelectedCategory != null)
                {
                    path = MetaclipApplication.Instance().LibraryPath + "\\" + this.SelectedCategory + "\\" + this.ClipName + ".clpx";
                }
                else
                {
                    path = MetaclipApplication.Instance().LibraryPath + "\\Uncategorized\\" + this.ClipName + ".clpx";
                }

                ClipMgr.StoreClip(path, Clip);

                DependencyObject view = System.Windows.Application.Current.MainWindow;
                IRegionManager regionManager = RegionManager.GetRegionManager(view);
                regionManager.RequestNavigate("MainRegion", "MainPanel");

                this.ClipName = "";
            }
            else
            {
                // To Do: Replace call to System.Windows.MessageBox with custom dialog service.
                MessageBox.Show("A clip with the specified name already exists. Add clip operation failed. Please specify a unique name and try again.", "Duplicate Clip Name Error");
            }
        }


        public ICommand NewCommand { get; private set; }
        private bool CanNew(object arg) { return true; }
        private void OnNew(object arg)
        {

            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "CategoryManager ");

            this.ClipName = "";
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set 
            {
                if (_selectedCategory == value)
                {
                    return;
                }
                _selectedCategory = value;
                RaisePropertyChanged("SelectedCategory");            
            }
        }

        private string _clipName;
        public string ClipName
        {
            get 
            { return _clipName; }
            set 
            {
                if (_clipName == value)
                {
                    return;
                }
                _clipName = value;
                RaisePropertyChanged("ClipName"); 
            }
        }

        private List<string> _categoryList;
        public List<string> CategoryList
        {
            get
            {
                return _categoryList;
            }
            set
            {
                _categoryList = value;
            }
        }

     }
}
