using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Practices.Prism.ViewModel;

namespace MetaClip.ViewModels
{    
    public abstract class ViewModelBase : NotificationObject, IDisposable 
    {
        public new event PropertyChangedEventHandler PropertyChanged;                       

        public string DisplayName
        {
            get;
            set;
        }

        public bool ThrowOnInvalidPropertyName
        {
            get;
            set;
        }

        protected virtual void OnPropertyChanged(string propertyName)        
        {
        
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public void VerifyPropertyName(string propertyName)        
        {

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        public void Dispose()
        {
        }

        protected void OnDispose()
        {
        }
    }
}
