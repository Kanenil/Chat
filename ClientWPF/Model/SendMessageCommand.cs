using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Commands;
using ClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Model
{
    public class SendMessageCommand : CommandBase
    {
        private readonly IService<MessageUserDTO> _messageUser;
        private readonly AccountViewModel _account;

        public SendMessageCommand(IService<MessageUserDTO> messageUser, AccountViewModel account)
        {
            _messageUser = messageUser;
            _account = account;
        }

        public async override void Execute(object parameter)
        {
            if(_account.Messages.Count == 0)
                _account.Messages.Add(new MessageModel()
                {
                    Message = _account.Message,
                    User = _account.User,
                    IsNativeOrigin = false,
                    Time = DateTime.Now,
                    ImageSource = _account.User.Photo,
                    FirstMessage = true
                });
            else
                _account.Messages.Add(new MessageModel()
                {
                    Message = _account.Message,
                    User = _account.User,
                    IsNativeOrigin = false,
                    Time = DateTime.Now,
                    ImageSource = _account.User.Photo,
                    FirstMessage = false
                });

            _account.SelectedContact.Messages = _account.Messages;

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


            _account.Message = "";
        }
    }
}
