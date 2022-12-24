using BusinnesLogicLayer.DTO;
using ClientWPF.Commands;
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
        public SettingsViewModel(INavigationService backNavigationService, AccountStore accountStore, INavigationService homeNavigationService)
        {
            BackCommand = new NavigateCommand(backNavigationService);
            LogoutCommand = new LogoutCommand(accountStore);
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            _account = accountStore;
        }
    }
}
