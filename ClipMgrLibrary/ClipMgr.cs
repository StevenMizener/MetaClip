using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.IO;

//**********************************************************************************************************
//**********************************************************************************************************
// NAME: Clip Manager Library
// VERSION: 0.1 (Pre-Beta) 
//
// DESCRIPTION: 
// A small class library that provides objects with methods useful for permanently storing, transferring     
// and retrieving the contents of the Windows system clipboard in a serialized binary file with the 
// extension ".clpx".
//**********************************************************************************************************
//**********************************************************************************************************

namespace ClipMgrLibrary
{ 
    [Serializable()]
    public class ClipFormats
    {
        public ClipFormats()
        {
        }
       
        private object _data;
        private string _format;

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
    }

    [Serializable()]
    public class Clip
    {
        private string _name;
        private string _category;
        private List<ClipFormats> _clipdata;

        public Clip()
        {
        } 

        public string ClipName
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public List<ClipFormats> ClipData
        {
            get { return _clipdata; }
            set { _clipdata = value; }
        }
    }

    public class ClipManager 
    {
        public ClipManager()
        {
        }

        public Boolean StoreClip(string filePath, Clip clip)
        {
            using (FileStream fs = new FileStream(filePath, System.IO.FileMode.OpenOrCreate))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, clip);
                return true;
            }
        }

        public Clip FetchClip(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, System.IO.FileMode.Open))
                    {
                        var Formatter = new BinaryFormatter();
                        Clip RestoredClip = new Clip();
                        object Obj = Formatter.Deserialize(fs);
                        RestoredClip = (Clip)Obj;
                        return RestoredClip;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public Clip MakeClip(string name, string category = "")
        {
            if (ValidateClip())
            {
                var MetaClip = new Clip();
                MetaClip.ClipName = name;
                MetaClip.Category = category;
                IDataObject data = Clipboard.GetDataObject();
                string[] Formats = data.GetFormats();
                List<object> objects = new List<object>();
                MetaClip.ClipData = new List<ClipFormats>(); 

                foreach (string Str in Formats)
                {
                    if (ValidateFormat(Str))
                    {
                        ClipFormats DF = new ClipFormats();
                        DF.Format = Str;
                        DF.Data = Clipboard.GetData(Str);
                        MetaClip.ClipData.Add(DF);
                    }
                }
                return MetaClip;
            }
            else
            {
                return null;
            }
        }

        public void RestoreClip(Clip clip)
        {
            try
            {
                Clipboard.Clear();
                IDataObject obj = new DataObject();
                List<ClipFormats> Data = clip.ClipData;

                foreach (ClipFormats DF in Data)
                {
                    obj.SetData(DF.Format, true, DF.Data);
                }
                Clipboard.SetDataObject(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Boolean ValidateClip() // Returns true if at least one format on clipboard is usable
        {
            IDataObject data = new DataObject(Clipboard.GetDataObject()); // Get data from clipboard
            Boolean result = false;
            if (data == null) // If data object is empty, then return false.
            {
                return false;
            }
            else
            {                
                foreach (string format in data.GetFormats()) // Loop through data formats on clipboard
                {
                    if (ValidateFormat(format) == true) // Validate each format against list of bad formats
                    {
                        result = true; // Set value of boolean to true if any one data format in object is valid
                    }
                }
            }
            return result; // Return validation result
        }

        private static Boolean ValidateFormat(string format) // Validates data formats against list of illegal formats
        {
            List<string> formats = new List<string>();

            // Add troublesome clipboard formats to list for validation
            formats.Add("EnhancedMetafile");
            formats.Add("Wk1");
            formats.Add("BIFF4");
            formats.Add("Biff");
            formats.Add("Biff3");
            formats.Add("Biff5");
            formats.Add("Biff8");
            formats.Add("Format129");
            formats.Add("DataInterchangeFormat");
            formats.Add("SymbolicLink");
            formats.Add("Link Source");
            formats.Add("Hyperlink");
            formats.Add("Link");
            formats.Add("Link Source Descriptor");
            formats.Add("ObjectLink");
            formats.Add("Object Descriptor");
            formats.Add("Embed Source");

            if (format == "") // Is item an empty string?
            {
                return false;
            }
            else if (formats.Contains(format)) // Is item in list of illegal formats?
            {
                return false; //If so, return false.
            }
            else // Otherwise, it's all good.
            {
                return true;
            }
        } 

    }
}
