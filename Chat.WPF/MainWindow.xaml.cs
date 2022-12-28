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
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 10;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - 10;
            _serverConnection = serverConnection;
        }
        private bool _canMove = false;
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                        Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    else
                        Application.Current.MainWindow.WindowState = WindowState.Normal;
                    return;
                }
                _canMove = true;
            }
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            if (_serverConnection.IsConnected)
                _serverConnection.CloseConnection();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _canMove)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    this.Top = e.GetPosition(this).Y - 15;
                }
                DragMove();
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _canMove = false;
        }
    }
}
