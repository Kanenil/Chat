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
    class ChangeUsernameCommand : AsyncCommandBase
    {
        private readonly ChangeUsernameViewModel _viewModel;
        private readonly UserStore _userStore;
        private readonly INavigationService _navigationService;

        public ChangeUsernameCommand(ChangeUsernameViewModel viewModel, UserStore userStore, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _userStore = userStore;
            _navigationService = navigationService;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _viewModel.PasswordMessage = "Password";
            _viewModel.UsernameMessage = "Username";
            _viewModel.PasswordMessageColor = "#808080";
            _viewModel.UsernameMessageColor = "#808080";

            if (!await Validate())
                return;

            User user = await _userStore.GetUserByLoginOrEmail(_viewModel.Username);
            if (user != null)
            {
                _viewModel.UsernameMessage = "Username - Current username is already registered!";
                _viewModel.UsernameMessageColor = "#c77377";
                return;
            }

            var updateUser = _userStore.LoginedUser;
            updateUser.Login = _viewModel.Username;

            _viewModel.PasswordMessage = "Password";
            _viewModel.UsernameMessage = "Username";
            _viewModel.PasswordMessageColor = "#808080";
            _viewModel.UsernameMessageColor = "#808080";
            _viewModel.Password = "";
            _viewModel.Username = updateUser.Login;


            await _userStore.UpdateUser(updateUser);

            _navigationService.Navigate();
        }

        private async Task<bool> Validate()
        {
            if (String.IsNullOrWhiteSpace(_viewModel.Username))
            {
                _viewModel.UsernameMessage = "Username - This field can`t be empty.";
                _viewModel.UsernameMessageColor = "#c77377";
                return false;
            }

            if (_viewModel.Username.Length > 20 && _viewModel.Username.Length < 4)
            {
                _viewModel.UsernameMessage = "Username - Username lenght can`t de over than 20 or less then 4 symbols.";
                _viewModel.UsernameMessageColor = "#c77377";
                return false;
            }


            if (String.IsNullOrWhiteSpace(_viewModel.Password))
            {
                _viewModel.PasswordMessage = "Password - The password is empty!";
                _viewModel.PasswordMessageColor = "#c77377";
                return false;
            }

            if (PasswordHasher.Hash(_viewModel.Password) != _userStore.LoginedUser.Password)
            {
                _viewModel.PasswordMessage = "Password - The passwords are different!";
                _viewModel.PasswordMessageColor = "#c77377";
                return false;
            }

            return true;
        }
    }
}
