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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace TImageViewer
{
    class ResizeVIewModel : INotifyPropertyChanged
    {
        MyImage _imageToResize;
        #region Properties
        private bool _preserveAspectRatio;
        public bool PreserveAspectRatio
        {
            get
            {
                return _preserveAspectRatio;
            }
            set
            {
                _preserveAspectRatio = value;
                OnPropertyChanged("PreserveAspectRatio");
            }
        }
        private double _width;
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width == value)
                    return;
                _width = value;
                ChangeHeight();
                OnPropertyChanged("Width");
            }
        }

        private double _height;
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height == value)
                    return;
                _height = value;
                ChangeWidth();
                OnPropertyChanged("Height");
            }
        }

        private Bitmap _resizedImage;
        public Bitmap ResizedImage
        {
            get
            {
                return _resizedImage;
            }
            set
            {
                _resizedImage = value;            
            }
        }
        private RelayCommand _save;
        public RelayCommand Save
        {
            get
            {
                return _save;
            }
        }

        #endregion
        public ResizeVIewModel(MyImage mainImage)
        {
            _imageToResize = mainImage;
            Width = _imageToResize.BitmapImage.Width;
            Height = _imageToResize.BitmapImage.Height;
            PreserveAspectRatio = true;
            _save = new RelayCommand(this.SaveOverImage, this.CanSaveOverImage);
        }

        void ChangeHeight()
        {
            if (PreserveAspectRatio)
            {
                double ratio = _imageToResize.BitmapImage.Height / _imageToResize.BitmapImage.Width;
                Height = Width * ratio;
            }
        }

        void ChangeWidth()
        {
            if (PreserveAspectRatio)
            {
                double ratio = _imageToResize.BitmapImage.Width / _imageToResize.BitmapImage.Height;
                Width = Height * ratio;             
            }
        }

        void SaveOverImage()
        {
            string imgFormat = "";
            Image toResize = Image.FromFile(_imageToResize.ImagePath);
            System.Drawing.Size size = new System.Drawing.Size((int)Width, (int)Height);
            ResizedImage = new Bitmap(toResize, size);

            int lastIndexOf = (_imageToResize.ImagePath.LastIndexOf("."));
            imgFormat = _imageToResize.ImagePath.Substring(lastIndexOf);
            imgFormat = imgFormat.ToLower();

            string saveFileName = _imageToResize.ImagePath.Substring(0, lastIndexOf) + "_resized";

            switch (imgFormat)
            {
                case ".jpg":
                case ".jpeg":
                    ResizedImage.Save(saveFileName + imgFormat, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case ".png":
                    ResizedImage.Save(saveFileName + imgFormat, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case ".bmp":
                    ResizedImage.Save(saveFileName + imgFormat, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case ".gif":
                    ResizedImage.Save(saveFileName + imgFormat, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                default:
                    ResizedImage.Save(saveFileName + imgFormat, System.Drawing.Imaging.ImageFormat.Png);
                    break;
            }         
            ResizedImage.Dispose();
            Window currWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);        
            currWindow.Close();
        }

        bool CanSaveOverImage()
        {
            if (Height != 0 && Width != 0 && _imageToResize != null)
            {
                return true;
            }
            else return false;
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
