﻿using Chat.WPF.Commands;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Server;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        #region Fields
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

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _emailText;
        public string EmailText
        {
            get { return _emailText; }
            set { _emailText = value; OnPropertyChanged(); }
        }

        private string _emailTextColor;
        public string EmailTextColor
        {
            get { return _emailTextColor; }
            set { _emailTextColor = value; OnPropertyChanged(); }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        private string _confirmPasswordText;
        public string ConfirmPasswordText
        {
            get { return _confirmPasswordText; }
            set { _confirmPasswordText = value; OnPropertyChanged(); }
        }

        private string _confirmPasswordTextColor;
        public string ConfirmPasswordTextColor
        {
            get { return _confirmPasswordTextColor; }
            set { _confirmPasswordTextColor = value; OnPropertyChanged(); }
        }
        #endregion

        public ICommand RegistrationCommand { get; }
        public ICommand CancelCommand { get; }

        public RegistrationViewModel(INavigationService backNavigationService, UserStore userStore, INavigationService homeNavigationService, ServerConnection serverConnection)
        {
            UsernameText = "Username";
            UsernameTextColor = "#FFB8BABD";
            EmailText = "Email address";
            EmailTextColor = "#FFB8BABD";
            PasswordText = "Password";
            PasswordTextColor = "#FFB8BABD";
            ConfirmPasswordText = "Confirm Password";
            ConfirmPasswordTextColor = "#FFB8BABD";

            CancelCommand = new NavigateCommand(backNavigationService);
            RegistrationCommand = new RegistrationCommand(this, userStore, homeNavigationService, serverConnection);
        }
    }
}
