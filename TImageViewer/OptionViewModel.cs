using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Configuration;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;

namespace TImageViewer
{
    class OptionViewModel : INotifyPropertyChanged
    {
        ObservableCollection<int> _ComboBoxRange = new ObservableCollection<int>();
        public ObservableCollection<int> ComboBoxRange
        {
            get
            {
                return _ComboBoxRange;
            }
        }
        #region AppProperties
        public bool IsFullScreenFlag
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsFullScreenFlag"]);            
            }
            set
            {
                AddUpdateAppSettings("IsFullScreenFlag", value.ToString());
                OnPropertyChanged("IsFullScreenFlag");
            }
        }
        public int SlideShowIntervalTime
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["IntervalTime"]);             
            }
            set
            {
                AddUpdateAppSettings("IntervalTime", value.ToString());
                OnPropertyChanged("SlideShowIntervalTime");
            }
        }


        #endregion



        public OptionViewModel()
        {
            for (int i = 1; i <= 100; i++)
            {
                _ComboBoxRange.Add(i);
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true)) 
                {
                    DateTime localDate = DateTime.Now;
                    file.WriteLine(localDate.ToString() + " " + "Error writing app settings");
                }
                            
            }
        }
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
