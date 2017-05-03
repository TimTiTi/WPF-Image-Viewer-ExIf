using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TImageViewer
{
    /// <summary>
    /// Interaction logic for FullScreenWindow.xaml
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        public FullScreenWindow(MyImage mainImage, ObservableCollection<string> images, int imageIndex)
        {
            InitializeComponent();
            DataContext = new FullScreenViewModel(mainImage, images,imageIndex);
        }
    
    }
}
