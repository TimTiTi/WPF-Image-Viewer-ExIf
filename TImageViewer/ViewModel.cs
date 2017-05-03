using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TImageViewer
{
   public class ViewModel : INotifyPropertyChanged
    {
        string[] args;
        int currentIndex;
        ObservableCollection<string> _images = new ObservableCollection<string>();
        #region Properties
        MyImage _mainMyImage;
        public MyImage MainMyImage
        {
            get { return _mainMyImage; }
            set
            {
                _mainMyImage = value;
                IndexTextBox = "change";
                ChangeMetadata(_mainMyImage);
                OnPropertyChanged("MainMyImage");
            }
        }
        ExIfMetadata _ExIfMain;
        public ExIfMetadata ExIfMain
        {
            get { return _ExIfMain; }
            set
            {
                _ExIfMain = value;
                OnPropertyChanged("ExIfMain");
            }
        }
        string _indexTextBox;
        public string IndexTextBox
        {
            get { return _indexTextBox; }
            set
            {
                _indexTextBox = currentIndex + 1 + "/" + _images.Count.ToString();
                OnPropertyChanged("IndexTextBox");
            }
        }
        
        #endregion
        #region RelayCommandProperties
        private RelayCommand _cmdPreviousImage;
        public RelayCommand CmdPreviousImage
        {
            get
            {
                return _cmdPreviousImage;
            }
        }
        private RelayCommand _cmdNextImage;
        public RelayCommand CmdNextImage
        {
            get
            {
                return _cmdNextImage;
            }
        }
        private RelayCommand _cmdPickImages;
        public RelayCommand CmdPickImages
        {
            get
            {
                return _cmdPickImages;
            }
        }
        private RelayCommand _rotateImageCW;
        public RelayCommand RotateImageCW
        {
            get
            {
                return _rotateImageCW;
            }
        }
        private RelayCommand _rotateImageCCW;
        public RelayCommand RotateImageCCW
        {
            get
            {
                return _rotateImageCCW;
            }
        }
        private RelayCommand _closeWidnow;
        public RelayCommand CloseWindow
        {
            get
            {
                return _closeWidnow;
            }
        }
        private RelayCommand _cmdFullScreen;
        public RelayCommand FullScreen
        {
            get
            {
                return _cmdFullScreen;
            }
        }
        private RelayCommand _about;
        public RelayCommand About
        {
            get
            {
                return _about;
            }
        }
        private RelayCommand _options;
        public RelayCommand Options
        {
            get
            {
                return _options;
            }
        }
        private RelayCommand _cmdResize;
        public RelayCommand CmdResize
        {
            get
            {
                return _cmdResize;
            }
        }
        #endregion
        public ViewModel()
        {
            _cmdPreviousImage = new RelayCommand(this.PreviousImage_Executed, this.PreviousImage_CanExecute);
            _cmdNextImage = new RelayCommand(this.NextImage_Executed, this.NextImage_CanExecute);
            _cmdPickImages = new RelayCommand(this.AddPhotosToView, this.CanAddPhotosToView);
            _rotateImageCW = new RelayCommand(this.RotateImageCW_Execute, this.CanRotateImage);
            _rotateImageCCW = new RelayCommand(this.RotateImageCCW_Execute, this.CanRotateImage);
            _closeWidnow = new RelayCommand(this.CloseWindow_Execute, this.CanCloseWindow);
            _cmdFullScreen = new RelayCommand(this.FullScreen_Execute, this.CanFullScreen);
            _about = new RelayCommand(this.About_Execute, this.CanAbout);
            _options = new RelayCommand(this.Options_Execute, this.CanOptions);
            _cmdResize = new RelayCommand(this.Resize_Execute,this.CanResize);
            CheckEnvironmentArgs();
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsFullScreenFlag"]))
                FullScreen_Execute();
       
                if (Directory.Exists("C:\\Users\\Public\\ITVC\\TImageViewer"))
                {
                    return;
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory("C:\\Users\\Public\\ITVC\\TImageViewer");
                }
        }
        public void CheckEnvironmentArgs()
        {
            args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                ProcessImageFolder();
        }
        private void ProcessImageFolder()
        {
            try
            {
                string imagePath = args[args.Length - 1];
                string folderPath = "";
                int lastIndexOf = (imagePath.LastIndexOf("\\"));
                folderPath = imagePath.Substring(0, lastIndexOf + 1);
                _images = GetImages(@folderPath);

                MainMyImage = new MyImage(_images.ElementAt(currentIndex));
            }
            catch (NotSupportedException)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Nije se moglo NotSupportedException");
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    DirectoryInfo di = Directory.CreateDirectory("C:\\Users\\Public\\ITVC\\TImageViewer");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Nije se moglo NotSupportedException");
                    }
                }
        }
            catch (NullReferenceException)
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Nije se moglo NullReferenceException");
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    DirectoryInfo di = Directory.CreateDirectory("C:\\Users\\Public\\ITVC\\TImageViewer");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Nije se moglo NullReferenceException");
                    }
                }
        }
        }
        /// <summary>
        /// Dobavlja sve datoteke sa ekstenzijom "extensions" i sprema ih u kolekciju, također postavlja za glavnu sliku  onu koja je pozvala program
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private ObservableCollection<string> GetImages(string directory)
        {
            ObservableCollection<string> images = new ObservableCollection<string>();
            string[] extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            int i = 0;

            DirectoryInfo di = new DirectoryInfo(directory);
            // FileInfo[] fi = di.GetFiles("*.jpg");
            FileInfo[] fi = di.GetFiles()
             .Where(f => extensions.Contains(f.Extension.ToLower()))
             .ToArray();

            foreach (FileInfo info in fi)
            {
                images.Add(info.FullName);
                if (args[args.Length - 1].Equals(info.FullName))
                    currentIndex = i++;
                else
                    i++;
            }
            return images;
        }
        void ChangeMetadata(MyImage currentImage)
        { // mjenja metadatau za trenutnu sliku
            ExIfMain = new ExIfMetadata(currentImage.ImagePath, currentImage.BitmapImage);
        }

        #region commands
        private void Options_Execute()
        {
            OptionWindow optionsWin = new OptionWindow();
            optionsWin.Show();
        }
        private bool CanResize()
        {
            if(MainMyImage != null)
            return true;
            return false;
        }
        private void Resize_Execute()
        {
            ResizeWindow resizeWin = new ResizeWindow(MainMyImage);
            resizeWin.Show();
        }
        private bool CanOptions()
        {
            return true;
        }
        private bool CanAbout()
        {
            return true;
        }
        private void About_Execute()
        {
            AboutWindow aboutWin = new AboutWindow();
            aboutWin.Show();
        }
        private bool CanCloseWindow()
        {
            return true;
        }
        private void CloseWindow_Execute()
        {
            Application.Current.MainWindow.Close();
        }
        private bool CanFullScreen()
        {
            return true;
        }
        private void FullScreen_Execute()
        {
            FullScreenWindow FCW = new FullScreenWindow(MainMyImage, _images, currentIndex);
            FCW.Show();
            Application.Current.MainWindow.Hide();
        }
        private void RotateImageCCW_Execute()
        {
            TransformedBitmap TempImage = new TransformedBitmap();

            TempImage.BeginInit();
            TempImage.Source = MainMyImage.BitmapImage; // MyImageSource of type BitmapImage
            MainMyImage.ImageRotation -= 90;
            RotateTransform transform = new RotateTransform(MainMyImage.ImageRotation);
            TempImage.Transform = transform;
            TempImage.EndInit();

            MainMyImage.ImageSource = TempImage;
            OnPropertyChanged("MainMyImage");
        }
        private void RotateImageCW_Execute()
        {
            TransformedBitmap TempImage = new TransformedBitmap();

            TempImage.BeginInit();
            TempImage.Source = MainMyImage.BitmapImage; // MyImageSource of type BitmapImage
            MainMyImage.ImageRotation += 90;
            RotateTransform transform = new RotateTransform(MainMyImage.ImageRotation);
            TempImage.Transform = transform;
            TempImage.EndInit();

            MainMyImage.ImageSource = TempImage;
            OnPropertyChanged("MainMyImage");
        }

        private bool CanRotateImage()
        {
            if (MainMyImage != null)
                return true;
            else
                return false;
        }
        private void NextImage_Executed()
        {
            MainMyImage = new MyImage(_images.ElementAt(++currentIndex));
        }

        private bool NextImage_CanExecute()
        {           
            if (currentIndex < (_images.Count - 1))
                return true;
            else
                return false;
        }

        private void PreviousImage_Executed()
        {
            MainMyImage = new MyImage(_images.ElementAt(--currentIndex));
        }

        private bool PreviousImage_CanExecute()
        {
            if (currentIndex == 0)
                return false;
            else
                return true;
        }
        private void AddPhotosToView() // za tipku za dodavanje slika -> dodaje samo označene slike
        {
            var photoFileDialog = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.exif|Joint Photographic Expert Group (*.jpg, *.jpeg)|*.jpg;*.jpeg|Portable Network Graphics (*.png)|*.png|Bitmap (*.bmp)|*.bmp|Graphics Interchange Format (*.gif)|*.gif|Exchangable Image File Format (*.exif)|*.exif",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Title = "Select Images to View", Multiselect = true };
            if (photoFileDialog.ShowDialog() == true)
            {
                ObservableCollection<string> images = new ObservableCollection<string>();
                foreach (String file in photoFileDialog.FileNames)
                {
                    images.Add(file);
                }
                _images = images;
                currentIndex = 0;
                MainMyImage = new MyImage(_images.ElementAt(currentIndex));
            }
        }
        private bool CanAddPhotosToView()
        {
            return true;
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
