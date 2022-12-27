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
    public class ChangeEmailViewModel : ViewModelBase
    {
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
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
        private string _emailMessage;
        public string EmailMessage { get => _emailMessage; set { _emailMessage = value; OnPropertyChanged(); } }
        private string _passwordMessageColor;
        public string PasswordMessageColor { get => _passwordMessageColor; set { _passwordMessageColor = value; OnPropertyChanged(); } }
        private string _emailMessageColor;
        public string EmailMessageColor { get => _emailMessageColor; set { _emailMessageColor = value; OnPropertyChanged(); } }
        public ICommand CancelCommand { get; }
        public ICommand DoneCommand { get; }

        public ChangeEmailViewModel(UserStore userStore, INavigationService backNavigationService)
        {
            PasswordMessage = "Password";
            EmailMessage = "Email";
            PasswordMessageColor = "#808080";
            EmailMessageColor = "#808080";

            Email = userStore.LoginedUser.Email;

            CancelCommand = new NavigateCommand(backNavigationService);
            DoneCommand = new ChangeEmailCommand(this, userStore, backNavigationService);
        }
    }
}
