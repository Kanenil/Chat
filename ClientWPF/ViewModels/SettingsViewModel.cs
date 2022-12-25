using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Commands;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly AccountStore _account;
        public UserDTO User => _account.CurrentAccount;
        public ICommand BackCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand NavigateHomeCommand { get; }
        public ICommand ChangePhotoCommand { get; }
        public ICommand ChangeUsernameCommand { get; }
        public SettingsViewModel(INavigationService backNavigationService, AccountStore accountStore,IService<UserDTO> service, INavigationService homeNavigationService, INavigationService changeNameNavigationService, ServerConnection server)
        {
            BackCommand = new NavigateCommand(backNavigationService);
            LogoutCommand = new LogoutCommand(accountStore, server);
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            ChangePhotoCommand = new ChangePhotoCommand(service, accountStore);
            ChangeUsernameCommand = new NavigateCommand(changeNameNavigationService);
            _account = accountStore;

            _account.CurrentAccountChanged += OnCurrentAccountChanged;
        }
        private void OnCurrentAccountChanged()
        {
            OnPropertyChanged("User");
        }

        public override void Dispose()
        {
            _account.CurrentAccountChanged -= OnCurrentAccountChanged;

            base.Dispose();
        }
    }
}
