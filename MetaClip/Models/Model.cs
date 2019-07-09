using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace MetaClip.Models
{       
    public class ClipLibrary
    {
        public ClipLibrary(string path)
        {
            InitializeCategories(path);
            SetCategory("", path);
        }

        public ClipLibrary(string path, string category)
        {
            InitializeCategories(path);
            SetCategory(category, path);
        }

        #region Static Methods // This region contains static methods of the clip library class

        public static void CreateDefaultFolders(string path)
        {
            if (!Directory.Exists(path + "\\Uncategorized"))
            {
                Directory.CreateDirectory(path + "\\Uncategorized");
            }
        }

        public static void CreateCategory(string path, string name)
        {
            if (!Directory.Exists(path + "\\" + name))
            {
                Directory.CreateDirectory(path + "\\" + name);
            }
        }

        public static void DeleteCategory(string path, string name)
        {
            if (Directory.Exists(path + "\\" + name))
            {
                Directory.Delete(path + "\\" + name,true);
            }
        }

        public static string GetPathbyName(string name, string path)
        {            
            string[] rtnpaths=Directory.GetFiles(path, name + ".clpx", SearchOption.AllDirectories);

            if (rtnpaths.Length > 1)
            {
                Exception ex = new Exception("Non-unique clip name error; more than one clip with the specified name found.");
                throw ex;
            }
            else if (rtnpaths.Length == 0)
            {
                Exception ex = new Exception("No clip selected. Please select a valid clip to restore.");
                throw ex;
            }
            return rtnpaths[0];
        }

        public static Boolean ValidateName(string name, string root)
        {
            string[] rtnpaths = Directory.GetFiles(root, name + ".clpx", SearchOption.AllDirectories);
            switch (rtnpaths.Length)
	        {
                case 0:
                    return true;
		        default:
                    return false;
	        }
        }

        public static void DeleteClipByName(string name, string root)
        {
            try
            {
                File.Delete(ClipLibrary.GetPathbyName(name, root));
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// To give credit where it's due, this code was swiped from MSDN article http://msdn.microsoft.com/en-us/library/bb762914.aspx.
        /// It's helpful of exporting the current MetaClip library.
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>        
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = System.IO.Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        } 

        #endregion

        #region Instance Methods // This region contains methods of the clip library class

        // Set the category filter as needed
        public string SetCategory(string category, string path)
        {
            string pathTrg = "";
            if (category != "All")
            {
                if (category != "")
                {
                    pathTrg = "\\" + category;
                    InitializeClipList(path + pathTrg);
                    this.CurrentPath = path + pathTrg;
                }
            }
            else
            {
                InitializeClipList(path, true);                
                this.CurrentPath = path;
            }
            return path + pathTrg;                        
        }
        
        // Initialize Categories
        private void InitializeCategories(string path)
        {
            var files = Directory.GetDirectories(path);
            List<string> SimpleNames = new List<string>();
            foreach (string str in files)
            {
                string[] str2 = str.Split('\\');
                SimpleNames.Add(str2[str2.Length - 1]);
            }
            this.Categories = SimpleNames;
        }

        // Initialize clip list with files in root of specified directory path
        private void InitializeClipList(string path)
        {
            var files = Directory.GetFiles(path, "*.clpx");
            List<string> SimpleNames = new List<string>();
            foreach (string str in files)
            {
                string[] str2 = str.Split('\\');
                string [] str3 = str2[str2.Length - 1].Split('.');

                SimpleNames.Add(str3[0]);
            }
            this.ClipList = SimpleNames;
        }

        // Overloaded method for initialize clip list; gets all files, including in subdirectories
        private void InitializeClipList(string path, Boolean allfileoption)
        {
            var files = Directory.GetFiles(path, "*.clpx", SearchOption.AllDirectories);
            List<string> SimpleNames = new List<string>();
            foreach (string str in files)
            {
                string[] str2 = str.Split('\\');
                string[] str3 = str2[str2.Length - 1].Split('.');

                SimpleNames.Add(str3[0]);
            }
            this.ClipList = SimpleNames;
        }

        // To-Do: Finish rewriting models to use in memory object to store file names, categories and paths.
        //private void InitializeLibrary()
        //{
        //    ClipFile ClpFile = new ClipFile();

        //}

        #endregion

        #region Properties // This region contains properties of the clip library class

        private string _currentPath;
        public string CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath = value; }
        }
        
        private List<ClipFile> _clipFiles;
        public List<ClipFile> ClipFiles
        {
            get { return _clipFiles; }
            set { _clipFiles = value; }
        }

        private List<string> _categories;
        public List<string> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        private List<string> _cliplist;
        public List<string> ClipList
        {
            get { return _cliplist; }
            set { _cliplist = value; }
        }

        #endregion  
    }
   
    public class ClipFile
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }      
    }

}
