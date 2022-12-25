using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Model;
using ClientWPF.Stores;
using ClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Commands
{
    public class SendMessageCommand : AsyncCommandBase
    {
        private readonly IMessageUserService<MessageUserDTO> _messageUser;
        private readonly AccountViewModel _account;
        private readonly ServerConnection _serverConnection;

        public SendMessageCommand(IMessageUserService<MessageUserDTO> messageUser, AccountViewModel account, ServerConnection serverConnection)
        {
            _messageUser = messageUser;
            _account = account;
            _serverConnection = serverConnection;
        }
        public override async Task ExecuteAsync(object parameter)
        {
            if (_account.Messages == null || string.IsNullOrWhiteSpace(_account.Message))
                return;

            await _messageUser.AddItemAsync(new MessageUserDTO()
            {
                FromUser = _account.User,
                ToUser = _account.SelectedContact.User,
                Message = new MessageDTO()
                {
                    Message = _account.Message,
                    Time = DateTime.Now,
                    User = _account.User
                }
            });

            OnMessageSended();
            if (_serverConnection.IsConnected)
                _serverConnection.SendMessage(_account.SelectedContact.User.Login, _account.User.Login);

            _account.Message = "";
        }
        public event Action MessageSended;
        private void OnMessageSended()
        {
            MessageSended?.Invoke();
        }
    }
}
