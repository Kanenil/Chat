using Chat.Domain.Models;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class RegistrationCommand : AsyncCommandBase
    {
        private readonly RegistrationViewModel _registrationViewModel;
        private readonly UserStore _userStore;
        private readonly INavigationService _navigation;
        private readonly ServerConnection _serverConnection;

        public RegistrationCommand(RegistrationViewModel registrationViewModel, UserStore userStore, INavigationService navigation, ServerConnection serverConnection)
        {
            _registrationViewModel = registrationViewModel;
            _userStore = userStore;
            _navigation = navigation;
            _serverConnection = serverConnection;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _registrationViewModel.IsLoading= true;

            _registrationViewModel.UsernameText = "Username";
            _registrationViewModel.UsernameTextColor = "#FFB8BABD";
            _registrationViewModel.EmailText = "Email address";
            _registrationViewModel.EmailTextColor = "#FFB8BABD";
            _registrationViewModel.PasswordText = "Password";
            _registrationViewModel.PasswordTextColor = "#FFB8BABD";
            _registrationViewModel.ConfirmPasswordText = "Confirm Password";
            _registrationViewModel.ConfirmPasswordTextColor = "#FFB8BABD";

            if (!await Validate())
                return;

            var user = new User(-1, _registrationViewModel.Username, _registrationViewModel.Email, PasswordHasher.Hash(_registrationViewModel.Password), null, false);

            try
            {
                await _userStore.Login(user);

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
                    _registrationViewModel.Username = "";
                    _registrationViewModel.Email = "";
                    _registrationViewModel.ConfirmPassword = "";
                    _registrationViewModel.Password = "";

                    _navigation.Navigate();
                    _registrationViewModel.IsLoading = false;
                }
            }
            catch 
            {


            }
        }

        private async Task<bool> Validate()
        {
            #region Email
            if (String.IsNullOrWhiteSpace(_registrationViewModel.Email))
            {
                _registrationViewModel.EmailText = "Email - This field can`t be empty.";
                _registrationViewModel.EmailTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }

            if (!Regex.IsMatch(_registrationViewModel.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                _registrationViewModel.EmailText = "Email - Email must be like \"something\"@\"domain\".";
                _registrationViewModel.EmailTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }

            if (_registrationViewModel.Email.Length > 50)
            {
                _registrationViewModel.EmailText = "Email - Email lenght can`t de over than 50 symbols.";
                _registrationViewModel.EmailTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }

            User userEmail = await _userStore.GetUserByLoginOrEmail(_registrationViewModel.Email);
            if (userEmail != null)
            {
                _registrationViewModel.EmailText = "Email - This email address is already registered.";
                _registrationViewModel.EmailTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }
            #endregion
            #region Username
            if (String.IsNullOrWhiteSpace(_registrationViewModel.Username))
            {
                _registrationViewModel.UsernameText = "Username - This field can`t be empty.";
                _registrationViewModel.UsernameTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }

            if (_registrationViewModel.Username.Length > 20 && _registrationViewModel.Username.Length < 4)
            {
                _registrationViewModel.UsernameText = "Username - Username lenght can`t de over than 20 or less then 4 symbols.";
                _registrationViewModel.UsernameTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }

            User userLogin = await _userStore.GetUserByLoginOrEmail(_registrationViewModel.Username);
            if (userLogin != null)
            {
                _registrationViewModel.EmailText = "Username - This username is already registered.";
                _registrationViewModel.EmailTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }
            #endregion
            #region Password
            if (String.IsNullOrWhiteSpace(_registrationViewModel.Password))
            {
                _registrationViewModel.PasswordText = "Password - This field can`t be empty.";
                _registrationViewModel.PasswordTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }

            if (_registrationViewModel.Password.Length < 6)
            {
                _registrationViewModel.PasswordText = "Password - Password lenght can`t de less then 6 symbols.";
                _registrationViewModel.PasswordTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }
            #endregion
            #region Confirm Password
            if (_registrationViewModel.Password != _registrationViewModel.ConfirmPassword)
            {
                _registrationViewModel.ConfirmPasswordText = "Confirm Password - Passwords aren`t equal.";
                _registrationViewModel.ConfirmPasswordTextColor = "#c77377";
                _registrationViewModel.IsLoading = false;
                return false;
            }
            #endregion
            return true;
        }
    }
}
