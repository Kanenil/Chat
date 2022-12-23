using ClientWPF.Commands;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using ClientWPF.ViewModels;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AccountStore accountStore, INavigationService loginNavigationService)
        {
            //LoginCommand = new LoginCommand(this, accountStore, loginNavigationService);
        }
    }
}
