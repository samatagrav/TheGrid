using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Grid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GridController gridController;

        public MainWindow()
        {
            gridController = new GridController(this);
            InitializeComponent();
            gridController.InnitGrid();
        }

        private void GridImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            gridController.LeftClick(e.GetPosition(this));
        }
        
        private void GridImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                gridController.WallClick(e.GetPosition(this));
            }
        }

        private Nullable<Point> _lastContextPosition;
        private void GridContextMenuOpened(object sender, RoutedEventArgs args) {
            ContextMenu menu = sender as ContextMenu;
            if (menu != null)
            {
                _lastContextPosition = Mouse.GetPosition(GridImage);
            }

        }

        private void SetStart(object sender, RoutedEventArgs args)
        {
            
            if (_lastContextPosition.HasValue)
            {
                gridController.SetStart(_lastContextPosition.Value);
                _lastContextPosition = null;
            }
        }

        private void SetEnd(object sender, RoutedEventArgs args)
        {
            if (_lastContextPosition.HasValue)
            {
                gridController.SetEnd(_lastContextPosition.Value);
                _lastContextPosition = null;

            }

        }

    }
}
