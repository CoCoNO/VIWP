using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VoyIteso.Resources;

namespace VoyIteso.Class
{
    class User
    {
        public String Token;
        public String key;
        public String usr;
        public String pass;
        public String Type;
        public String Name;
        public String completeName;
        public String profileID;
        public String isLocationAllowed;
        public String gender;
        public String imageUrl;

        BitmapImage avatar;
        public BitmapImage Avatar
        {
            get {
                return avatar;
            }


            set 
            {
                avatar = value;
                OnUserDataChanged(EventArgs.Empty);
                
            }
        }
        /*
        private byte[] userImage;
        public byte[] UserImage
        {
            get { return userImage; }
            set
            {
                userImage = value;
            }
        }*/

        //public event PropertyChangedEventHandler PropertyChanged;

        public User()
        {
            //this.key = "VoyItesoKeyToken";
            
        }
        /*
        #region NotifyPropertyChanged
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        */

        #region Events
        public EventHandler UserDataChanged;
        protected virtual void OnUserDataChanged(EventArgs e)
        {


            EventHandler handler = UserDataChanged;
            if (handler != null)
            {
                handler(this, e);
            }
            //clear resources
        }
        #endregion
        #region getInfo
        public void getInfo(String key)
        {
            String[] values = new String[5];

            /*if (settings.Contains(key))
            {
                values = (String[])settings[key];

                if (values[0] != null)
                {
                    Token = values[0];
                }
                else
                    Token = null;

                if (values[1] != null)
                {
                    if (values[1] == "driver")
                        Type = "driver";
                    else
                        Type = "walker";
                }
                else
                    Type = null;
                if (values[2] != null)
                {
                    Name = values[2];
                }
                else
                    Name = null;
                if (values[3] != null)
                {
                    profileID = values[3];
                }
                else
                    profileID = null;
                if (values[4] != null)
                {
                    isLocationAllowed = values[4];
                }
                else
                    isLocationAllowed = "false";
            }
            else
            {
                Token = null;
                Type = null;
                isLocationAllowed = "false";
            }*/
        }
        #endregion

        #region setInfo
        public void setInfo(String key)
        {
            String[] value = new String[5];
            value[0] = Token;
            value[1] = Type;
            value[2] = Name;
            value[3] = profileID;
            value[4] = isLocationAllowed;

            /*if (settings.Contains(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key,value);
            }
            settings.Save();*/
        }
        #endregion

        #region deleteInfo
        public void deleteInfo(String key)
        {
            /*if (settings.Contains(key))
            {
                settings.Clear();
            }*/
        }
        #endregion

        #region setImageUrl
        public bool setImageUrl()
        {
            if (profileID != null)
            {
                imageUrl = AppResources.ApiBaseUrl + AppResources.ApiGetProfileImage;
                imageUrl = String.Format(imageUrl, profileID);
                return true;
            }
            else
                return false;
        }
        #endregion

        public void setLocationPermission(bool permission)
        {
            if (permission)
                this.isLocationAllowed = "true";
            else
                this.isLocationAllowed = "false";
        }

        public bool getLocationAllowed() 
        {
            if (this.isLocationAllowed.Equals("true"))
                return true;
            else
                return false;
        }

    }
}
