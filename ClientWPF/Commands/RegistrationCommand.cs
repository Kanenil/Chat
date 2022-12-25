using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
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
            if (_viewModel.ConfirmPassword != _viewModel.Password)
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
    }
}
