using Chat.Domain.Models;
using Chat.WPF.Commands.Base;
using Chat.WPF.Convertors;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class ChangeEmailCommand : AsyncCommandBase
    {
        private readonly ChangeEmailViewModel _viewModel;
        private readonly UserStore _userStore;
        private readonly INavigationService _navigationService;

        public ChangeEmailCommand(ChangeEmailViewModel viewModel, UserStore userStore, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _userStore = userStore;
            _navigationService = navigationService;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _viewModel.PasswordMessage = "Password";
            _viewModel.EmailMessage = "Email";
            _viewModel.PasswordMessageColor = "#808080";
            _viewModel.EmailMessageColor = "#808080";

            if (!await Validate())
                return;

            User user = await _userStore.GetUserByLoginOrEmail(_viewModel.Email);
            if (user != null)
            {
                _viewModel.EmailMessage = "Email - Current email is already registered!";
                _viewModel.EmailMessageColor = "#c77377";
                return;
            }

            var updateUser = _userStore.LoginedUser;
            updateUser.Email = _viewModel.Email;

            _viewModel.PasswordMessage = "Password";
            _viewModel.EmailMessage = "Email";
            _viewModel.PasswordMessageColor = "#808080";
            _viewModel.EmailMessageColor = "#808080";
            _viewModel.Password = "";
            _viewModel.Email = updateUser.Email;


            await _userStore.UpdateUser(updateUser);

            _navigationService.Navigate();
        }

        private async Task<bool> Validate()
        {
            if (String.IsNullOrWhiteSpace(_viewModel.Email))
            {
                _viewModel.EmailMessage = "Username - This field can`t be empty.";
                _viewModel.EmailMessageColor = "#c77377";
                return false;
            }

            if (_viewModel.Email.Length > 20 && _viewModel.Email.Length < 4)
            {
                _viewModel.EmailMessage = "Username - Username lenght can`t de over than 20 or less then 4 symbols.";
                _viewModel.EmailMessageColor = "#c77377";
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
