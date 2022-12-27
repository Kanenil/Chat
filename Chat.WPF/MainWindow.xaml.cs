using Chat.WPF.Server;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
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

namespace Chat.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerConnection _serverConnection;
        public MainWindow(ServerConnection serverConnection)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _serverConnection = serverConnection;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            var icon = new PackIconMaterial();
            icon.Width = 20;
            icon.Height = 20;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.Foreground = Brushes.Gray;
            icon.Padding = new Thickness(4);

            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                icon.Kind = PackIconMaterialKind.DockWindow;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                icon.Kind = PackIconMaterialKind.WindowMaximize;
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }

            (sender as Button).Content = icon;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            if (_serverConnection.IsConnected)
                _serverConnection.CloseConnection();
        }
    }
}
