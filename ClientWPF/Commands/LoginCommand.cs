using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Converters;
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
            _viewModel.UsernameText = "Email address or Login";
            _viewModel.UsernameTextColor = "#FFB8BABD";
            _viewModel.PasswordText = "Password";
            _viewModel.PasswordTextColor = "#FFB8BABD";
            _viewModel.UsernameRequiredVisibilty = Visibility.Visible;
            _viewModel.PasswordRequiredVisibilty = Visibility.Visible;

            if (String.IsNullOrWhiteSpace(_viewModel.Username))
            {
                _viewModel.UsernameRequiredVisibilty = Visibility.Collapsed;
                _viewModel.UsernameText = "Email address or Login - This is a required field.";
                _viewModel.UsernameTextColor = "#c77377";
                return;
            }

            if(String.IsNullOrWhiteSpace(_viewModel.Password))
            {
                _viewModel.PasswordRequiredVisibilty = Visibility.Collapsed;
                _viewModel.PasswordText = "Password - This is a required field.";
                _viewModel.PasswordTextColor = "#c77377";
                return;
            }


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
                _viewModel.UsernameRequiredVisibilty = Visibility.Collapsed;
                _viewModel.UsernameText = "Email address or Login - Invalid login or email.";
                _viewModel.UsernameTextColor = "#c77377";

                return;
            }
            var user = _service.GetAll().First(u=>u.Id == id);
            var password = PasswordHasher.Hash(_viewModel.Password);


            if (user.Password != password) 
            {
                _viewModel.PasswordRequiredVisibilty = Visibility.Collapsed;
                _viewModel.PasswordText = "Password - Invalid password.";
                _viewModel.PasswordTextColor = "#c77377";
                return;
            }

            _accountStore.CurrentAccount = user;

            _navigationService.Navigate();
        }
    }
}
