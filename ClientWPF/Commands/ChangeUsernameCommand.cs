using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using ClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Commands
{
    public class ChangeUsernameCommand : AsyncCommandBase
    {
        private readonly IService<UserDTO> _service;
        private readonly AccountStore _accountStore;
        private readonly INavigationService _navigationService;
        private readonly ChangeUsernameViewModel _model;

        public ChangeUsernameCommand(IService<UserDTO> service, AccountStore accountStore, INavigationService navigationService, ChangeUsernameViewModel model)
        {
            _service = service;
            _accountStore = accountStore;
            _navigationService = navigationService;
            _model = model;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                if (PasswordHasher.Hash(_model.Password) != _accountStore.CurrentAccount.Password)
                {
                    _model.PasswordMessage = "Password - The passwords are different!";
                    _model.PasswordMessageColor = "#FF0000";
                    return;
                }
            }
            catch
            {
                _model.PasswordMessage = "Password - The passwords are empty!";
                _model.PasswordMessageColor = "#FF0000";
                return;
            }

            try
            {
                _service.GetAll().First(u => u.Login == _model.Username);
                _model.UsernameMessage = "Username - Current username is already registered!";
                _model.UsernameMessageColor = "#FF0000";
                return;
            }
            catch
            {
                var user = _accountStore.CurrentAccount;
                user.Login = _model.Username;
                await _service.UpdateDTO(user);

                _navigationService.Navigate();
            }
        }
    }
}
