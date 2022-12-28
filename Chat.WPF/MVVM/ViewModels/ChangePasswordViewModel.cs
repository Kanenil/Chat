using Chat.WPF.Commands;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
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

        private string _newPassword;
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }
        private string _newPasswordMessage;
        public string NewPasswordMessage { get => _newPasswordMessage; set { _newPasswordMessage = value; OnPropertyChanged(); } }
        private string _newPasswordMessageColor;
        public string NewPasswordMessageColor { get => _newPasswordMessageColor; set { _newPasswordMessageColor = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
        private string _confirmPasswordMessage;
        public string ConfirmPasswordMessage { get => _confirmPasswordMessage; set { _confirmPasswordMessage = value; OnPropertyChanged(); } }
        private string _confrimPasswordMessageColor;
        public string ConfirmPasswordMessageColor { get => _confrimPasswordMessageColor; set { _confrimPasswordMessageColor = value; OnPropertyChanged(); } }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged("IsLoading"); }
        }


        public ICommand CancelCommand { get; }
        public ICommand DoneCommand { get; }

        public ChangePasswordViewModel(UserStore userStore, INavigationService backNavigationService)
        {
            PasswordMessage = "Current Password";
            NewPasswordMessage = "New Password";
            ConfirmPasswordMessage = "Confirm New Password";
            PasswordMessageColor = "#808080";
            NewPasswordMessageColor = "#808080";
            ConfirmPasswordMessageColor = "#808080";

            CancelCommand = new NavigateCommand(backNavigationService);
            DoneCommand = new ChangePasswordCommand(this, userStore, backNavigationService);
        }

    }
}
