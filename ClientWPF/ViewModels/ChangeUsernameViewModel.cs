using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
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
    public class ChangeUsernameViewModel : ViewModelBase
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
        private string _passwordMessage;
        public string PasswordMessage { get => _passwordMessage; set { _passwordMessage = value; OnPropertyChanged(); } }
        private string _usernameMessage;
        public string UsernameMessage { get => _usernameMessage; set { _usernameMessage = value; OnPropertyChanged(); } }
        private string _passwordMessageColor;
        public string PasswordMessageColor { get => _passwordMessageColor; set { _passwordMessageColor = value; OnPropertyChanged(); } }
        private string _usernameMessageColor;
        public string UsernameMessageColor { get => _usernameMessageColor; set { _usernameMessageColor = value; OnPropertyChanged(); } }
        public ICommand CancelCommand { get; }
        public ICommand DoneCommand { get; }
        public ChangeUsernameViewModel(IService<UserDTO> service, AccountStore account, INavigationService backNavigationService)
        {
            PasswordMessage = "Password";
            UsernameMessage = "Username";
            PasswordMessageColor = "#808080";
            UsernameMessageColor = "#808080";

            Username = account.CurrentAccount.Login;
            DoneCommand = new ChangeUsernameCommand(service, account, backNavigationService, this);
            CancelCommand = new NavigateCommand(backNavigationService);
        }
    }
}
