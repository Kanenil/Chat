using Chat.Domain.Models;
using Chat.WPF.Commands.Base;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Server;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class SendMessageCommand : AsyncCommandBase
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly UserStore _userStore;
        private readonly ServerConnection _serverConnection;

        public SendMessageCommand(HomeViewModel homeViewModel, UserStore userStore, ServerConnection serverConnection)
        {
            _homeViewModel = homeViewModel;
            _userStore = userStore;
            _serverConnection = serverConnection;
        }

        public event Action MessageSended;

        public async override Task ExecuteAsync(object parameter)
        {
            if (_homeViewModel.Messages == null || string.IsNullOrWhiteSpace(_homeViewModel.Message) || _homeViewModel.SelectedContact == null)
                return;

            await _userStore.SendMessage(new Domain.Models.MessageUser()
            {
                FromUser = _userStore.LoginedUser,
                ToUser = _homeViewModel.SelectedContact.User,
                Message = new Message(-1, _homeViewModel.Message, DateTime.Now, _userStore.LoginedUser)
            });

            MessageSended?.Invoke();
            if (_serverConnection.IsConnected)
                await _serverConnection.SendMessage(_homeViewModel.SelectedContact.User.Login, _homeViewModel.User.Login);

            _homeViewModel.Message = "";
        }
    }
}
