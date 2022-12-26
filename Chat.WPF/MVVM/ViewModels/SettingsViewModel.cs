using Chat.Domain.Models;
using Chat.WPF.Commands;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Server;
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
    public class SettingsViewModel : ViewModelBase
    {
        private readonly UserStore _userStore;
        public User User { get; set; }
        public ICommand BackCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ChangePhotoCommand { get; }
        public ICommand ChangeUsernameCommand { get; }

        public SettingsViewModel(UserStore userStore, INavigationService homeNavigationService, INavigationService loginNavigationService, INavigationService changeUsernameNavigationService, ServerConnection serverConnection)
        {
            _userStore = userStore;
            User = _userStore.LoginedUser;
            _userStore.LoginedUserChanged += LoginedUserChanged;

            BackCommand = new NavigateCommand(homeNavigationService);
            LogoutCommand = new LogoutCommand(loginNavigationService, serverConnection);
            ChangeUsernameCommand = new NavigateCommand(changeUsernameNavigationService);
            ChangePhotoCommand = new ChangePhotoCommand(userStore);
        }

        private void LoginedUserChanged()
        {
            User = _userStore.LoginedUser;
            OnPropertyChanged(nameof(User));
        }
        public override void Dispose()
        {
            _userStore.LoginedUserChanged -= LoginedUserChanged;
            base.Dispose();
        }
    }
}
