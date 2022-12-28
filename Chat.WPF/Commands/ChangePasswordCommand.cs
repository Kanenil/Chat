using Chat.Domain.Models;
using Chat.WPF.Commands.Base;
using Chat.WPF.Convertors;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class ChangePasswordCommand : AsyncCommandBase
    {
        private readonly ChangePasswordViewModel _viewModel;
        private readonly UserStore _userStore;
        private readonly INavigationService _navigationService;

        public ChangePasswordCommand(ChangePasswordViewModel viewModel, UserStore userStore, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _userStore = userStore;
            _navigationService = navigationService;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _viewModel.IsLoading = true;
            ClearText();

            if (!Validate())
                return;

            var updateUser = _userStore.LoginedUser;
            updateUser.Password = PasswordHasher.Hash(_viewModel.NewPassword);

            ClearText();
            _viewModel.Password = "";
            _viewModel.NewPassword = "";
            _viewModel.ConfirmPassword = "";

            await _userStore.UpdateUser(updateUser);
            _navigationService.Navigate();
        }

        private bool Validate()
        {
            #region Password
            if (String.IsNullOrWhiteSpace(_viewModel.Password))
            {
                _viewModel.PasswordMessage = "Password - This field can`t be empty.";
                _viewModel.PasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return false;
            }

            if (PasswordHasher.Hash(_viewModel.Password) != _userStore.LoginedUser.Password)
            {
                _viewModel.PasswordMessage = "Password - The passwords are different.";
                _viewModel.PasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return false;
            }
            #endregion
            #region New Password
            if (String.IsNullOrWhiteSpace(_viewModel.NewPassword))
            {
                _viewModel.NewPasswordMessage = "New Password - This field can`t be empty.";
                _viewModel.NewPasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return false;
            }

            if (PasswordHasher.Hash(_viewModel.NewPassword) == _userStore.LoginedUser.Password)
            {
                _viewModel.NewPasswordMessage = "New Password - Password is the same as it was.";
                _viewModel.NewPasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return false;
            }

            if (_viewModel.NewPassword.Length < 6)
            {
                _viewModel.NewPasswordMessage = "New Password - Password lenght can`t de less then 6 symbols.";
                _viewModel.NewPasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return false;
            }
            #endregion
            #region Confirm Password
            if (_viewModel.NewPassword != _viewModel.ConfirmPassword)
            {
                _viewModel.ConfirmPasswordMessage = "Confirm New Password - Passwords aren`t equal.";
                _viewModel.ConfirmPasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return false;
            }
            #endregion
            return true;
        }

        private void ClearText()
        {
            _viewModel.PasswordMessage = "Current Password";
            _viewModel.NewPasswordMessage = "New Password";
            _viewModel.ConfirmPasswordMessage = "Confirm New Password";
            _viewModel.PasswordMessageColor = "#808080";
            _viewModel.NewPasswordMessageColor = "#808080";
            _viewModel.ConfirmPasswordMessageColor = "#808080";
        }
    }
}
