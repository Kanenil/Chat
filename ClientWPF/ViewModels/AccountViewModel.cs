using ClientWPF.Commands;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly AccountStore _accountStore;

        public string Username => _accountStore.CurrentAccount?.Login;
        public string Email => _accountStore.CurrentAccount?.Email;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSettingsCommand { get; }

        public AccountViewModel(AccountStore accountStore, INavigationService homeNavigationService, INavigationService settingsNavigationService)
        {
            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);

            _accountStore.CurrentAccountChanged += OnCurrentAccountChanged;
        }

        private void OnCurrentAccountChanged()
        {
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Username));
        }

        public override void Dispose()
        {
            _accountStore.CurrentAccountChanged -= OnCurrentAccountChanged;

            base.Dispose();
        }
    }
}
