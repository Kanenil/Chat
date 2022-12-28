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
    public class DeleteAccountCommand : AsyncCommandBase
    {
        private readonly DeleteUserViewModel _viewModel;
        private readonly UserStore _userStore;
        private readonly LogoutCommand _logoutCommand;

        public DeleteAccountCommand(DeleteUserViewModel viewModel, UserStore userStore, LogoutCommand logout)
        {
            _viewModel = viewModel;
            _userStore = userStore;
            _logoutCommand = logout;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _viewModel.IsLoading = true;

            _viewModel.PasswordMessage = "Password";
            _viewModel.PasswordMessageColor = "#808080";

            if (String.IsNullOrWhiteSpace(_viewModel.Password))
            {
                _viewModel.PasswordMessage = "Password - This field can`t be empty.";
                _viewModel.PasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return;
            }

            if (PasswordHasher.Hash(_viewModel.Password) != _userStore.LoginedUser.Password)
            {
                _viewModel.PasswordMessage = "Password - The passwords are different.";
                _viewModel.PasswordMessageColor = "#c77377";
                _viewModel.IsLoading = false;
                return;
            }

            _viewModel.PasswordMessage = "Password";
            _viewModel.PasswordMessageColor = "#808080";
            _viewModel.Password = "";

            await _userStore.DeleteUser(_userStore.LoginedUser.Id);
            _logoutCommand.Execute(null);

            _viewModel.IsLoading = false;
        }
    }
}
