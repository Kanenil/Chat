﻿using Chat.Domain.Models;
using Chat.WPF.Commands.Base;
using Chat.WPF.Convertors;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Server;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.WPF.Commands
{
    public class LoginCommand : AsyncCommandBase
    {
        private readonly LoginViewModel _loginViewModel;
        private readonly UserStore _userStore;
        private readonly INavigationService _navigation;
        private readonly ServerConnection _serverConnection;

        public LoginCommand(LoginViewModel loginViewModel, UserStore userStore, INavigationService navigation, ServerConnection serverConnection)
        {
            _loginViewModel = loginViewModel;
            _userStore = userStore;
            _navigation = navigation;
            _serverConnection = serverConnection;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            TextFieldsDefault();

            if (String.IsNullOrWhiteSpace(_loginViewModel.Username))
            {
                _loginViewModel.UsernameRequiredVisibilty = Visibility.Collapsed;
                _loginViewModel.UsernameText = "Email address or Login - This is a required field.";
                _loginViewModel.UsernameTextColor = "#c77377";
                return;
            }

            if (String.IsNullOrWhiteSpace(_loginViewModel.Password))
            {
                _loginViewModel.PasswordRequiredVisibilty = Visibility.Collapsed;
                _loginViewModel.PasswordText = "Password - This is a required field.";
                _loginViewModel.PasswordTextColor = "#c77377";
                return;
            }

            User user = await _userStore.GetUserByLoginOrEmail(_loginViewModel.Username);
            if (user == null)
            {
                _loginViewModel.UsernameRequiredVisibilty = Visibility.Collapsed;
                _loginViewModel.UsernameText = "Email address or Login - Invalid login or email.";
                _loginViewModel.UsernameTextColor = "#c77377";
                return;
            }

            var password = PasswordHasher.Hash(_loginViewModel.Password);

            if (user.Password != password)
            {
                _loginViewModel.PasswordRequiredVisibilty = Visibility.Collapsed;
                _loginViewModel.PasswordText = "Password - Invalid password.";
                _loginViewModel.PasswordTextColor = "#c77377";
                return;
            }

            TextFieldsDefault();

            _userStore.LoginedUser = user;
            try
            {
                await _serverConnection.Connect();
                await _serverConnection.Rename(user.Login);
                await _serverConnection.GetAllConnectedUsers(user.Login);
            }
            catch 
            {

            }
            finally
            {
                _loginViewModel.Username = "";
                _loginViewModel.Password = "";

                _navigation.Navigate();
            }
        }
        private void TextFieldsDefault()
        {
            _loginViewModel.UsernameText = "Email address or Login";
            _loginViewModel.UsernameTextColor = "#FFB8BABD";
            _loginViewModel.PasswordText = "Password";
            _loginViewModel.PasswordTextColor = "#FFB8BABD";
            _loginViewModel.UsernameRequiredVisibilty = Visibility.Visible;
            _loginViewModel.PasswordRequiredVisibilty = Visibility.Visible;
        }
    }
}