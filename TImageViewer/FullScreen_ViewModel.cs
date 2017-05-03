using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TImageViewer
{
    class FullScreen_ViewModel : INotifyPropertyChanged
    {
        int currentIndex;
        ObservableCollection<MyImage> _images = new ObservableCollection<MyImage>();
        #region Properties
        MyImage _mainMyImage;
        public MyImage MainMyImage
        {
            get { return _mainMyImage; }
            set
            {
                _mainMyImage = value;             
                OnPropertyChanged("MainMyImage");
            }
        }
        #endregion
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
