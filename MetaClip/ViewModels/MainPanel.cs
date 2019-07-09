using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MetaClip.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions; 

namespace MetaClip.ViewModels
{
    class MainPanel : ViewModelBase, INavigationAware
    {
        #region Constructor
        public MainPanel()
        {
            try
            {
                this.SubmitCommand = new DelegateCommand<object>(this.OnSubmit, this.CanSubmit);
                this.SaveCommand = new DelegateCommand<object>(this.OnSave, this.CanSave);
                this.RestoreCommand = new DelegateCommand<object>(this.OnRestore, this.CanRestore);
                this.CloseCommand = new DelegateCommand<object>(this.OnClose, this.CanClose);
                this.DeleteCommand = new DelegateCommand<object>(this.OnDelete, this.CanDelete);
                this.LaunchCatMgrCommand = new DelegateCommand<object>(this.OnLaunchCatMgr, this.CanLaunchCatMgr);

                InitializeClipLibrary();                
             }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        # region Methods
        public void InitializeClipLibrary()
        {
            MetaclipApplication.Instance().State.SetLibrary();

            this.ClipFiles = MetaclipApplication.Instance().ClipLibrary.ClipList;
            this.CategoryList = MetaclipApplication.Instance().ClipLibrary.Categories;
            this.CategoryList.Insert(0, "All");
            if (MetaclipApplication.Instance().FilterCategory != null)
            {
                this.SelectedCategory = MetaclipApplication.Instance().FilterCategory;
            }
            RaisePropertyChanged("ClipFiles");
            RaisePropertyChanged("CategoryList");
        }
        #endregion

        #region Commands
        public ICommand SubmitCommand { get; private set; }
        private bool CanSubmit(object arg) { return true; }
        private void OnSubmit(object arg)   
        {
            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "LibrarySettings");
        }

        public ICommand SaveCommand { get; private set; }
        private bool CanSave(object arg) { return true; }
        private void OnSave(object arg)
        {
            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "AddClip");
        }

        public ICommand RestoreCommand { get; private set; }
        private bool CanRestore(object arg) { return true; }
        private void OnRestore(object arg)
        {
            try
            {
                string path = Models.ClipLibrary.GetPathbyName(SelectedClip, MetaclipApplication.Instance().LibraryPath);
                MetaclipApplication.Instance().State.RestoreClip(path);
            }
            catch (Exception)
            {
                return;
            }
        }

        public ICommand CloseCommand { get; private set; }
        private bool CanClose(object arg) { return true; }
        private void OnClose(object arg)
        {
            System.Windows.Application.Current.MainWindow.Close();
        }

        public ICommand DeleteCommand { get; private set; }
        private bool CanDelete(object arg) { return true; }
        private void OnDelete(object arg)
        {
            try
            {
                ClipLibrary.DeleteClipByName(this.SelectedClip, MetaclipApplication.Instance().LibraryPath);
                ClipLibrary lib = new ClipLibrary(MetaclipApplication.Instance().LibraryPath, this.SelectedCategory);
                this.ClipFiles = lib.ClipList;
                RaisePropertyChanged("ClipFiles");

            }
            catch (Exception e)
            {
                // To-do: Replace messagebox function with custom dialog service.
                MessageBox.Show(e.Message);
            }
        }
        
        public ICommand LaunchCatMgrCommand { get; private set; }
        private bool CanLaunchCatMgr(object arg) { return true; }
        private void OnLaunchCatMgr(object arg)
        {
            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "CategoryManager");
        }
        #endregion

        #region Properties
        private string selectedCategory;
        public string SelectedCategory
        {
            get { return selectedCategory; }

            set
            {
                if (selectedCategory == value)
                    return;
                selectedCategory = value;

                // Publish event(s) when the selected item changes 
                RaisePropertyChanged("SelectedCategory");

                // Now filter clip list by new category
                ClipLibrary lib = new ClipLibrary(MetaclipApplication.Instance().LibraryPath, this.SelectedCategory);
                this.ClipFiles = lib.ClipList;

                // And publish the event to update the listview bound to this property
                RaisePropertyChanged("ClipFiles");
            }
        }

        private string selectedClip;
        public string SelectedClip
        {
            get { return selectedClip; }

            set
            {
                if (selectedClip == value)
                    return;
                selectedClip = value;

                // Publish event(s) when the selected item changes 
                RaisePropertyChanged("SelectedClip");
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

        private List<string> _clipFiles;
        public List<string> ClipFiles
        {
            get
            {
                return _clipFiles;
            }
            set
            {
                _clipFiles = value;
            }
        }
        #endregion

        #region INavigationAware Implementation
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext){}

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            MetaclipApplication.Instance().State.SetLibrary();
            InitializeClipLibrary();
            
        }
        #endregion 
    }
}
