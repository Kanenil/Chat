using Chat.WPF.Commands;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class DeleteUserViewModel : ViewModelBase
    {
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
        private string _passwordMessageColor;
        public string PasswordMessageColor { get => _passwordMessageColor; set { _passwordMessageColor = value; OnPropertyChanged(); } }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }


        public ICommand CancelCommand { get; }
        public ICommand DoneCommand { get; }

        public DeleteUserViewModel(UserStore userStore, INavigationService backNavigationService, LogoutCommand logout)
        {
            PasswordMessage = "Current Password";
            PasswordMessageColor = "#808080";

            CancelCommand = new NavigateCommand(backNavigationService);
            DoneCommand = new DeleteAccountCommand(this, userStore, logout);
        }
    }
}
