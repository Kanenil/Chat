using Chat.WPF.Commands;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Server;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _usernameText;
        public string UsernameText
        {
            get { return _usernameText; }
            set { _usernameText = value; OnPropertyChanged(); }
        }

        private string _usernameTextColor;
        public string UsernameTextColor
        {
            get { return _usernameTextColor; }
            set { _usernameTextColor = value; OnPropertyChanged(); }
        }

        private string _passwordText;
        public string PasswordText
        {
            get { return _passwordText; }
            set { _passwordText = value; OnPropertyChanged(); }
        }

        private string _passwordTextColor;
        public string PasswordTextColor
        {
            get { return _passwordTextColor; }
            set { _passwordTextColor = value; OnPropertyChanged(); }
        }

        private Visibility _usernameRequiredVisibilty;
        public Visibility UsernameRequiredVisibilty
        {
            get { return _usernameRequiredVisibilty; }
            set { _usernameRequiredVisibilty = value; OnPropertyChanged(); }
        }

        private Visibility _passwordRequiredVisibilty;
        public Visibility PasswordRequiredVisibilty
        {
            get { return _passwordRequiredVisibilty; }
            set { _passwordRequiredVisibilty = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegistrationCommand { get; }

        public LoginViewModel(INavigationService registartionNavigationService, UserStore userStore, INavigationService homeNavigationService, ServerConnection serverConnection)
        {
            UsernameText = "Email address or Login";
            UsernameTextColor = "#FFB8BABD";
            PasswordText = "Password";
            PasswordTextColor = "#FFB8BABD";
            UsernameRequiredVisibilty = Visibility.Visible;
            PasswordRequiredVisibilty = Visibility.Visible;

            RegistrationCommand = new NavigateCommand(registartionNavigationService);
            LoginCommand = new LoginCommand(this, userStore, homeNavigationService, serverConnection);
        }
    }
}
