using System.Windows.Forms;
using MetaClip.Properties;
using MetaClip.Models;
using System.IO;

namespace MetaClip.ViewModels
{   
    ///SUMMARY: Application state management logic and infrastructure below.
    ///This code defines the application context, combining the singleton pattern
    ///and the state pattern into an application-level class that manages state 
    ///and provides common operations. Includes abstract classes for states 
    ///and their derived concrete types referenced as the state property of the 
    ///application context object.
    
    #region Application Context Classes and Implementations

    // Application context class (singleton)
    public class MetaclipApplication
    {        
        private static MetaclipApplication instance = null;
        private ApplicationState state;
        public ApplicationState State
        {
            get {return state; }
            set { state = value; }
        }

        private ClipLibrary clipLibrary;
        public ClipLibrary ClipLibrary
        {
            get { return clipLibrary; }
            set { clipLibrary = value; }           
        }

        private string libraryPath;
        public string LibraryPath
        {
            get { return libraryPath; }
            set { libraryPath = value; }
        }



        private string filterCategory;
        public string FilterCategory
        {
            get
            {
                if (filterCategory == "")
                    filterCategory = Settings.Default.FilterCategory;
                return filterCategory; 
            }
            set 
            {                 
                filterCategory = value;
                Settings.Default.FilterCategory = filterCategory;
                Settings.Default.Save();
            }
        }

        public static MetaclipApplication Instance()
        {
            if (instance == null)
            {
                instance = new MetaclipApplication();
            }
            return instance;
        }

        protected MetaclipApplication()
        {
            Initializing InitialState = new Initializing();
            this.State = InitialState;
        }
     }

    #endregion   
   
    #region Application States 

    // Application state management code begins here...
    public abstract class ApplicationState  // Abstract base class for application states
    {
        public abstract void SetLibrary(); 
        public abstract string StatusMessage { get; set;}
        public abstract void RestoreClip(string path);
        protected virtual bool ValidateLibrary()
        {
            if (Directory.Exists(Settings.Default.LibraryLocation))
            {
                return (true);
            }
            else
            {
                LibraryMissing NewState = new LibraryMissing();
                MetaclipApplication.Instance().State = NewState;
                return (false);
            }
        }
        protected virtual bool ValidateCategory(string category)
        {
            if (Directory.Exists(Settings.Default.LibraryLocation + "\\" + category))
            {
                if (category != "")
                    return (true);
                return (false);
            }
            else
            {
                return (false);
            }
        }

        public virtual void ExportLibrary()
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowNewFolderButton = true;
            fb.Description = "Select an export location for the MetaClip library. All files and subfolders will be copied to the selected location.";

            if (fb.ShowDialog() == DialogResult.OK)
            {
                Models.ClipLibrary.DirectoryCopy(Settings.Default.LibraryLocation,fb.SelectedPath,true);
            }

            fb.Dispose();
        }

        public virtual void ChangeLibrary()
        { 
            FolderBrowserDialog fb = new FolderBrowserDialog();            
            fb.ShowNewFolderButton = true;            
            fb.Description = "Please select an initial location for the MetaClip library. You can change this location later.";
            
            if (fb.ShowDialog() == DialogResult.OK)            
            {            
                Settings.Default.LibraryLocation = fb.SelectedPath;               
                Settings.Default.Save();                
                MetaclipApplication.Instance().LibraryPath = Settings.Default.LibraryLocation;                
                Models.ClipLibrary.CreateDefaultFolders(Settings.Default.LibraryLocation);                
                MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, "All");                
                MetaclipApplication.Instance().State = new Initialized();                
            }

            fb.Dispose();
        }
   
    }

    // State of application when first launching
    public class Initializing : ApplicationState
    {        
        private string _statusMessage = "Initializing";
        public override string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = "Initializing"; }
        }
              
        public override void SetLibrary()
        {            
            if (this.ValidateLibrary())
            {
                MetaclipApplication.Instance().LibraryPath = Settings.Default.LibraryLocation;                
                if (this.ValidateCategory(Settings.Default.FilterCategory))
                {
                    MetaclipApplication.Instance().FilterCategory = Settings.Default.FilterCategory;
                    MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, Settings.Default.FilterCategory);
                }
                else
                {
                    MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, "All");
                }

                MetaclipApplication.Instance().State = new Initialized();               

            }
            else
            {
                FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.ShowNewFolderButton = true;
                fb.Description = "Please select an initial location for the MetaClip library. You can change this location later.";

                if (fb.ShowDialog() == DialogResult.OK)
                {
                    Settings.Default.LibraryLocation = fb.SelectedPath;
                    Settings.Default.Save();
                    MetaclipApplication.Instance().LibraryPath = Settings.Default.LibraryLocation;
                    Models.ClipLibrary.CreateDefaultFolders(Settings.Default.LibraryLocation);
                    MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, "All");
                    MetaclipApplication.Instance().State = new Initialized(); 
                }
                else
                {
                    MetaclipApplication.Instance().State=new LibraryMissing();
                }

                fb.Dispose();
            }      
        }

        public override void RestoreClip(string path)
        {
            return;
        }
    }

    // State of application when library successfully initialized
    public class Initialized : ApplicationState
    {
        private string _statusMessage = "Initialized";
        public override string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = "Initialized";
            }
        }

        public override void SetLibrary()
        {

            if (this.ValidateCategory(Settings.Default.FilterCategory))
            {
                MetaclipApplication.Instance().FilterCategory = Settings.Default.FilterCategory;
                MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, Settings.Default.FilterCategory);
            }
            else
            {            
                // Refresh library            
                MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, "All");                
            }            
        }

        public override void RestoreClip(string path)
        {                          
            // Restore stored clip to clipboard
            MetaclipApplication.Instance().State = new RestoringClip();
            MetaclipApplication.Instance().State.RestoreClip(path);            
        }                 
    }

    // State of application while a clip is being restored
    public class RestoringClip : ApplicationState
    {
        public override void SetLibrary()
        {
            // Don't change horses in the middle of the race
            return;
        }

        public override void RestoreClip(string path)
        {
            ClipMgrLibrary.ClipManager ClipMgr = new ClipMgrLibrary.ClipManager();
            ClipMgrLibrary.Clip clip = ClipMgr.FetchClip(path);
            ClipMgr.RestoreClip(clip);
            MetaclipApplication.Instance().State = new Initialized(); 
        }

        private string _statusMessage = "Restoring clip.";
        public override string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = "Restoring clip.";
            }
        }
    }

    // State of application if load library fails
    public class LibraryMissing : ApplicationState
    {
        private string _statusMessage = "Library Missing";
        public override string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = "Library Missing"; }
        }

        public override void SetLibrary()
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowNewFolderButton = true;
            fb.Description = "Please select a valid location for the MetaClip library. You can change this location again at any time.";
            if (fb.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.LibraryLocation = fb.SelectedPath;
                Settings.Default.Save();

                Models.ClipLibrary.CreateDefaultFolders(Settings.Default.LibraryLocation);
                MetaclipApplication.Instance().ClipLibrary = new Models.ClipLibrary(Settings.Default.LibraryLocation, "All");
                MetaclipApplication.Instance().LibraryPath = Settings.Default.LibraryLocation;

                // Set application state to initialized
                Initialized appstate = new Initialized();
                MetaclipApplication.Instance().State = appstate;                
            }
            else
            {
                fb.Dispose();
                return;
            }
            fb.Dispose();
        }

        public override void RestoreClip(string path)
        {
            return;
        }
    }

    #endregion
 



}
