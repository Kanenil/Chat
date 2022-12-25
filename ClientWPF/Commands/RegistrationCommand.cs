using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
using ClientWPF.Converters;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using ClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ClientWPF.Commands
{
    public class RegistrationCommand : AsyncCommandBase
    {
        private readonly RegistrationViewModel _viewModel;
        private readonly AccountStore _accountStore;
        private readonly INavigationService _navigationService;
        private readonly IService<UserDTO> _userService;
        public RegistrationCommand(RegistrationViewModel viewModel, AccountStore accountStore,IService<UserDTO> userService, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _accountStore = accountStore;
            _navigationService = navigationService;
            _userService = userService;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _viewModel.UsernameText = "Username";
            _viewModel.UsernameTextColor = "#FFB8BABD";
            _viewModel.EmailText = "Email address";
            _viewModel.EmailTextColor = "#FFB8BABD";
            _viewModel.PasswordText = "Password";
            _viewModel.PasswordTextColor = "#FFB8BABD";
            _viewModel.ConfirmPasswordText = "Confirm Password";
            _viewModel.ConfirmPasswordTextColor = "#FFB8BABD";

            if (!await Validate())
                return;

            var user = new UserDTO()
            {
                Email = _viewModel.Email,
                Password = PasswordHasher.Hash(_viewModel.Password),
                Login = _viewModel.Username,
                EmailConfirmed = false
            };


            await _userService.AddItemAsync(user);
            _accountStore.CurrentAccount = _userService.GetAll().First(u => u.Login == _viewModel.Username);

            _navigationService.Navigate();
        }

        private async Task<bool> Validate()
        {
            #region Email
            if (String.IsNullOrWhiteSpace(_viewModel.Email))
            {
                _viewModel.EmailText = "Email - This field can`t be empty.";
                _viewModel.EmailTextColor = "#c77377";
                return false;
            }

            if (!Regex.IsMatch(_viewModel.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                _viewModel.EmailText = "Email - Email must be like \"something\"@\"domain\".";
                _viewModel.EmailTextColor = "#c77377";
                return false;
            }

            if (_viewModel.Email.Length > 100)
            {
                _viewModel.EmailText = "Email - Email lenght can`t de over than 100 symbols.";
                _viewModel.EmailTextColor = "#c77377";
                return false;
            }

            try
            {
                var user = (await _userService.GetAllAsync()).First(u => u.Email.ToLower() == _viewModel.Email.ToLower());
                _viewModel.EmailText = "Email - This email address is already registered.";
                _viewModel.EmailTextColor = "#c77377";
                return false;
            }
            catch { }
            #endregion
            #region Username
            if (String.IsNullOrWhiteSpace(_viewModel.Username))
            {
                _viewModel.UsernameText = "Username - This field can`t be empty.";
                _viewModel.UsernameTextColor = "#c77377";
                return false;
            }

            if (_viewModel.Username.Length > 20 && _viewModel.Username.Length < 4)
            {
                _viewModel.UsernameText = "Username - Username lenght can`t de over than 20 or less then 4 symbols.";
                _viewModel.UsernameTextColor = "#c77377";
                return false;
            }

            try
            {
                var user = (await _userService.GetAllAsync()).First(u => u.Login.ToLower() == _viewModel.Username.ToLower());
                _viewModel.UsernameText = "Username - This username is already registered.";
                _viewModel.UsernameTextColor = "#c77377";
                return false;
            }
            catch { }
            #endregion
            #region Password
            if (String.IsNullOrWhiteSpace(_viewModel.Password))
            {
                _viewModel.PasswordText = "Password - This field can`t be empty.";
                _viewModel.PasswordTextColor = "#c77377";
                return false;
            }

            if (_viewModel.Username.Length < 6)
            {
                _viewModel.PasswordText = "Password - Password lenght can`t de less then 6 symbols.";
                _viewModel.PasswordTextColor = "#c77377";
                return false;
            }
            #endregion
            #region Confirm Password
            if (_viewModel.Password != _viewModel.ConfirmPassword)
            {
                _viewModel.ConfirmPasswordText = "Confirm Password - Passwords aren`t equal.";
                _viewModel.ConfirmPasswordTextColor = "#c77377";
                return false;
            }
            #endregion
            return true;
        }
    }
}
