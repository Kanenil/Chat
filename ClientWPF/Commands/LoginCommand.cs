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
using System.Windows;

namespace ClientWPF.Commands
{
    public class LoginCommand : CommandBase
    {
        private readonly HomeViewModel _viewModel;
        private readonly AccountStore _accountStore;
        private readonly INavigationService _navigationService;
        private readonly IService<UserDTO> _service;

        public LoginCommand(HomeViewModel viewModel, AccountStore accountStore, IService<UserDTO> service, INavigationService navigationService)
        {
            _viewModel = viewModel;
            _accountStore = accountStore;
            _navigationService = navigationService;
            _service = service;
        }

        public override void Execute(object parameter)
        {
            _viewModel.Message = "";

            int id;
            try
            {
                id = _service.GetIdDTO(new UserDTO()
                {
                    Email = _viewModel.Username,
                    Login = _viewModel.Username
                });
            }
            catch 
            {
                _viewModel.Message = "* Wrong login or email!";
                return;
            }
            var user = _service.GetAll().First(u=>u.Id == id);
            var password = PasswordHasher.Hash(_viewModel.Password);


            if (user.Password != password) { _viewModel.Message = "* Wrong password!"; return; }

            _viewModel.Message = "";


            _accountStore.CurrentAccount = user;

            _navigationService.Navigate();
        }
    }
}
