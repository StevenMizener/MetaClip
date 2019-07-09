using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;


namespace MetaClip.ViewModels
{
    class CategoryManager : ViewModelBase, INavigationAware
    { 
        public CategoryManager()
        {
            this.CloseCommand = new DelegateCommand<object>(this.OnClose, this.CanClose);
            this.AddCommand = new DelegateCommand<object>(this.OnAdd, this.CanAdd);
            this.SetDefaultCommand = new DelegateCommand<object>(this.OnSetDefault, this.CanSetDefault);
            this.DeleteCommand = new DelegateCommand<object>(this.OnDelete, this.CanDelete);
        }

        #region Commands

        public ICommand CloseCommand { get; private set; }
        private bool CanClose(object arg) { return true; }
        private void OnClose(object arg)
        {
            DependencyObject view = System.Windows.Application.Current.MainWindow;
            IRegionManager regionManager = RegionManager.GetRegionManager(view);
            regionManager.RequestNavigate("MainRegion", "MainPanel");
        }


        public ICommand AddCommand { get; private set; }
        private bool CanAdd(object arg) { return true; }
        private void OnAdd(object arg)
        {
            Models.ClipLibrary.CreateCategory(MetaclipApplication.Instance().LibraryPath, this.NewCategory);
            MetaclipApplication.Instance().State.SetLibrary();
            this.CategoryList=MetaclipApplication.Instance().ClipLibrary.Categories;     
        }

        public ICommand SetDefaultCommand { get; private set; }
        private bool CanSetDefault(object arg) { return true; }
        private void OnSetDefault(object arg)
        {
            MetaclipApplication.Instance().FilterCategory = this.SelectedCategory;
        }

        public ICommand DeleteCommand { get; private set; }
        private bool CanDelete(object arg) { return true; }
        private void OnDelete(object arg)
        {
            Models.ClipLibrary.DeleteCategory(MetaclipApplication.Instance().LibraryPath, this.SelectedCategory);
            MetaclipApplication.Instance().State.SetLibrary();
            this.CategoryList = MetaclipApplication.Instance().ClipLibrary.Categories;
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
            }
        }

        private List<string> categoryList;
        public List<string> CategoryList
        {
            get
            {
                return categoryList;
            }
            set
            {
                if (value == categoryList)
                    return;
                categoryList = value;
                RaisePropertyChanged("CategoryList");
            }
        }

        private string newCategory;
        public string NewCategory
        {
            get { return newCategory; }
            set {         
                if (value == newCategory)
                    return;
                newCategory = value;
                RaisePropertyChanged("NewCategory");
            }
        }
               
        #endregion

        #region INavigationAware implementation
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // put finalization code here
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.NewCategory = "";
            this.CategoryList = MetaclipApplication.Instance().ClipLibrary.Categories;
        }
        #endregion

    }

}
