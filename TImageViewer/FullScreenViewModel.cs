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
    class FullScreenViewModel : INotifyPropertyChanged
    {
        int currentIndex;
        private DispatcherTimer timerImageChange;
        ObservableCollection<string> _images;
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
        MyImage _fadeOutImage;
        public MyImage FadeOutImage
        {
            get { return _fadeOutImage; }
            set
            {
                _fadeOutImage = value;
                OnPropertyChanged("FadeOutImage");
            }
        }
        Visibility _playerVisibility;
        public Visibility PlayerVisibility
        {
            get { return _playerVisibility; }
            set
            {
                _playerVisibility = value;
                OnPropertyChanged("PlayerVisibility");
            }
        }
        Visibility _menuVisibility;
        public Visibility MenuVisibility
        {
            get { return _menuVisibility; }
            set
            {
                _menuVisibility = value;
                OnPropertyChanged("MenuVisibility");
            }
        }
        #endregion
        #region RelayCommandProperties
        private RelayCommand _closeWidnow;
        public RelayCommand CloseWindow
        {
            get
            {
                return _closeWidnow;
            }
        }
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
        private RelayCommand _runSlideShow;
        public RelayCommand RunSlideShow
        {
            get
            {
                return _runSlideShow;
            }
        }
        private RelayCommand _StopSlideShow;
        public RelayCommand StopSlideShow
        {
            get
            {
                return _StopSlideShow;
            }
        }
        private RelayCommand _mouseMove;
        public RelayCommand MouseMove
        {
            get
            {
                return _mouseMove;
            }
        }
        private RelayCommand _closeApplication;
        public RelayCommand CloseApplication
        {
            get
            {
                return _closeApplication;
            }
        }
        public RelayCommand<MouseEventArgs> MoveMouseCommand
        {
            get;
            private set;
        }
        #endregion
        public FullScreenViewModel(MyImage mainImage, ObservableCollection<string> images, int imageIndex)
        {
            _mainMyImage = mainImage;
            _images = images;
            currentIndex = imageIndex;
            _playerVisibility = Visibility.Collapsed;
            _menuVisibility = Visibility.Collapsed;
            _cmdPreviousImage = new RelayCommand(this.PreviousImage_Executed, this.PreviousImage_CanExecute);
            _cmdNextImage = new RelayCommand(this.NextImage_Executed, this.NextImage_CanExecute);
            _closeWidnow = new RelayCommand(this.CloseWindow_Execute, this.CanCloseWindow);
            _closeApplication = new RelayCommand(this.CloseApplication_Executed, this.CanCloseApplication);
            _runSlideShow = new RelayCommand(this.SlideShow_Execute, this.CanRunSlideShow);
            _StopSlideShow = new RelayCommand(this.StopSlideShow_Execute, this.CanStopSlideShow);
            _mouseMove = new RelayCommand(this.MoveMouse_Executed, this.CanMoveMouse);
            MoveMouseCommand = new RelayCommand<MouseEventArgs>(e =>
            {
                var element = e.OriginalSource as UIElement;
                //var point = e.GetPosition(element);              
                Window currWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                var point = Mouse.GetPosition(currWindow);
                double difference = (currWindow.ActualHeight * 0.10f);
                double borderHeight = currWindow.ActualHeight - difference;
                if (point.Y >= borderHeight)
                {
                    PlayerVisibility = Visibility.Visible;
                }
                else if (point.Y < borderHeight)
                {
                    PlayerVisibility = Visibility.Collapsed;
                    if (point.Y < difference)
                    {
                        MenuVisibility = Visibility.Visible;
                    }
                    else
                    {
                        MenuVisibility = Visibility.Collapsed;
                    }
                }
                Console.WriteLine("Position: {0}x{1}", point.X, point.Y);
            });
        }

        private void timerImageChange_Tick(object sender, EventArgs e)
        {        
            PlaySlideShow();
        }
        private void PlaySlideShow()
        {
            if (NextImage_CanExecute())
                NextImage_Executed();
        }

        #region Command_Execute
        private void StopSlideShow_Execute()
        {
            timerImageChange.IsEnabled = false;
        }
        private void SlideShow_Execute()
        {
            int intervalTimer = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalTime"]); ;
            timerImageChange = new DispatcherTimer();
            timerImageChange.Interval = new TimeSpan(0, 0, intervalTimer);
            timerImageChange.Tick += new EventHandler(timerImageChange_Tick);
            timerImageChange.IsEnabled = true;
        }
        private void CloseWindow_Execute()
        {
            Window currWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if(timerImageChange != null)
            timerImageChange.IsEnabled = false;
            Application.Current.MainWindow.Show();
            currWindow.Close();
        }
        private void PreviousImage_Executed()
        {
            MainMyImage = new MyImage(_images.ElementAt(--currentIndex));
        }
             
        private void NextImage_Executed()
        {
            MainMyImage =new MyImage(_images.ElementAt(++currentIndex));
        }
        private void MoveMouse_Executed()
        {
            //Console.WriteLine("Radi! ! !");
        }
        private void CloseApplication_Executed()
        {
            Application.Current.Shutdown();
        }
        #endregion
        #region Can_Execute
        private bool CanMoveMouse()
        {
            return true;
        }
        private bool CanStopSlideShow()
        {
            if (timerImageChange != null)
                return true;
            else
                return false;
        }
        private bool CanRunSlideShow()
        {
            if (_images.Count > 1)
                return true;
            else
                return false;
        }
        private bool CanCloseWindow()
        {
            return true;
        }
        private bool CanCloseApplication()
        {
            return true;
        }
        private bool PreviousImage_CanExecute()
        {
            if (currentIndex == 0)
                return false;
            else
                return true;
        }
        private bool NextImage_CanExecute()
        {
            if (currentIndex < (_images.Count - 1))
                return true;
            else
                return false;
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
