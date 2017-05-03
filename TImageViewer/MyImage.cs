using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TImageViewer
{
   public class MyImage
    {
        private string _imagePath;
        private Uri _uriImage;
        private BitmapImage _bitmapImage;
        private ImageSource _imageSource;
        private double _imageRotation;
        // private ExIfMetadata _exifMetadata;
        public MyImage(string imagePath)
        {
            _imagePath = imagePath;
            _uriImage = new Uri(imagePath);
            // _exifMetadata = new ExIfMetadata(imagePath);
            try
            {
                _bitmapImage = new BitmapImage(_uriImage);
                _imageSource = _bitmapImage;
                _imageRotation = 0;
            }
            catch (System.NotSupportedException)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Format nije podrzan! -> MyImage: " + imagePath);
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    DirectoryInfo di = Directory.CreateDirectory("C:\\Users\\Public\\ITVC\\TImageViewer");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Direktorij kreiran! " );
                    }
                }
            }
           
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }
        public Uri UriImage
        {
            get { return _uriImage; }
            set { _uriImage = value; }
        }
        //public ExIfMetadata ExIfMetadata
        //{
        //    get { return _exifMetadata; }
        //    set { _exifMetadata = value; }
        //}
        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set { _bitmapImage = value; }
        }
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value; }
        }
        public double ImageRotation
        {
            get { return _imageRotation; }
            set { _imageRotation = value; }
        }
    }
}
