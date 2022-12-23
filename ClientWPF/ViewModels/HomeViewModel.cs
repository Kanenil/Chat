using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Commands;
using ClientWPF.Services;
using ClientWPF.Stores;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
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
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegistrationCommand { get; }

        public HomeViewModel(AccountStore accountStore,IService<UserDTO> service, INavigationService loginNavigationService, INavigationService registartionNavigationService)
        {
            LoginCommand = new LoginCommand(this, accountStore, service, loginNavigationService);
            RegistrationCommand = new NavigateCommand(registartionNavigationService);
        }
    }
}
