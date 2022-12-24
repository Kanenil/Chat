using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
using ClientWPF.Commands;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly IService<MessageUserDTO> _messageService;
        private readonly IService<UserDTO> _userService;
        private readonly AccountStore _accountStore;
        private readonly ServerConnection _serverConnection;
        public UserDTO User => _accountStore.CurrentAccount;
        private ObservableCollection<ContactModel> _contacts;

        public ObservableCollection<ContactModel> Contacts
        {
            get { return _contacts; }
            set { _contacts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<MessageModel> _messages;

        public ObservableCollection<MessageModel> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }
        private string _selectedLogin;

        private ContactModel _selectedContact;
        public ContactModel SelectedContact 
        { 
            get => _selectedContact; 
            set { _selectedContact = value; if (_selectedContact != null) { Messages = _selectedContact.Messages; _selectedLogin = _selectedContact.User.Login; } OnPropertyChanged(); } 
        }
        public string Online { get; set; }
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand SendCommand { get; }

        public AccountViewModel(AccountStore accountStore, INavigationService homeNavigationService, INavigationService settingsNavigationService, IService<UserDTO> service, IService<MessageUserDTO> messageUserService, ServerConnection serverConnection)
        {
            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
            _userService = service;
            _messageService = messageUserService;
            Online = "Status Online";
            _serverConnection = serverConnection;

            try
            {
                _serverConnection.Connect();

                _serverConnection.DataReceived += ServerConnection_DataReceived;

                _serverConnection.Rename(_accountStore.CurrentAccount.Login);
            }
            catch (System.Exception ex)
            {
                Online = "Status Offline";
            }

            ConfigureChat(messageUserService, service);

            SendCommand = new SendMessageCommand(messageUserService, this, serverConnection);

            _accountStore.CurrentAccountChanged += OnCurrentAccountChanged;
        }

        private void ServerConnection_DataReceived()
        {
            ConfigureChat(_serverConnection.LastMessage);
        }

        private void ConfigureChat(IService<MessageUserDTO> messageUserService, IService<UserDTO> service)
        {
            var allMessages = messageUserService.GetAll();

            Contacts = new ObservableCollection<ContactModel>();
            foreach (var item in service.GetAll())
            {
                if (item.Id != _accountStore.CurrentAccount.Id)
                {
                    var messages = allMessages.Where(m => (m.FromUser.Id == _accountStore.CurrentAccount.Id && m.ToUser.Id == item.Id) || (m.ToUser.Id == _accountStore.CurrentAccount.Id && m.FromUser.Id == item.Id)).ToList();
                    messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
                    ObservableCollection<MessageModel> messages1 = new ObservableCollection<MessageModel>();
                    foreach (var message in messages)
                    {
                        messages1.Add(new MessageModel()
                        {
                            User = message.FromUser,
                            ImageSource = message.FromUser.Photo,
                            Username = message.FromUser.Login,
                            IsNativeOrigin = false, //message.ToUser.Id != _accountStore.CurrentAccount.Id ? false : true,
                            FirstMessage = messages1.Count == 0 ? true : messages1.Last().User.Id != message.FromUser.Id ? true : false,
                            Message = message.Message.Message,
                            Time = message.Message.Time
                        });
                    }

                    Contacts.Add(new ContactModel()
                    {
                        User = item,
                        Messages = messages1
                    });
                }
            }
            var list = Contacts.ToList();
            list.Sort(0, Contacts.Count,
                Comparer<ContactModel>.Create((a, b) =>
                {
                    if (a.Messages.Count > 0 && b.Messages.Count > 0)
                        return a.Messages.Last().Time < b.Messages.Last().Time ? 1 : a.Messages.Last().Time > b.Messages.Last().Time ? -1 : 0;
                    return a.Messages.Count > 0 ? -1 : 1;
                }));

            Contacts.Clear();
            foreach (var item in list)
                Contacts.Add(item);
        }

        private void ConfigureChat(string login)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedContact = null;

                var allMessages = _messageService.GetAll();
                ContactModel cont = new ContactModel();

                var login = _serverConnection.LastMessage;

                foreach (var contact in Contacts)
                {
                    if (contact.User.Login == login)
                    {
                        cont = contact;
                        break;
                    }
                }

                var messages = allMessages.Where(m => (m.FromUser.Id == _accountStore.CurrentAccount.Id && m.ToUser.Id == cont.User.Id) || (m.ToUser.Id == _accountStore.CurrentAccount.Id && m.FromUser.Id == cont.User.Id)).ToList();
                messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
                var lastMessage = messages.Last();
                cont.Messages.Add(new MessageModel()
                {
                    User = lastMessage.FromUser,
                    ImageSource = lastMessage.FromUser.Photo,
                    Username = lastMessage.FromUser.Login,
                    IsNativeOrigin = false, //message.ToUser.Id != _accountStore.CurrentAccount.Id ? false : true,
                    FirstMessage = cont.Messages.Count == 0 ? true : cont.Messages.Last().User.Id != lastMessage.FromUser.Id ? true : false,
                    Message = lastMessage.Message.Message,
                    Time = lastMessage.Message.Time
                });

                var contacts = new List<ContactModel>();
                foreach (var contact in Contacts)
                {
                    if (contact.User.Login == login)
                    {
                        contacts.Add(cont);
                        continue;
                    }
                    contacts.Add(contact);
                }

                contacts.Sort(0, contacts.Count,
                    Comparer<ContactModel>.Create((a, b) =>
                    {
                        if (a.Messages.Count > 0 && b.Messages.Count > 0)
                            return a.Messages.Last().Time < b.Messages.Last().Time ? 1 : a.Messages.Last().Time > b.Messages.Last().Time ? -1 : 0;
                        return a.Messages.Count > 0 ? -1 : 1;
                    }));

                Contacts = new ObservableCollection<ContactModel>();

                foreach (var item in contacts)
                    Contacts.Add(item);

                SelectedContact = Contacts.Where(x => x.User.Login == _selectedLogin).First();
            });
        }

        private void OnCurrentAccountChanged()
        {
            OnPropertyChanged("User");
        }

        public override void Dispose()
        {
            _accountStore.CurrentAccountChanged -= OnCurrentAccountChanged;
            _serverConnection.DataReceived -= ServerConnection_DataReceived;

            base.Dispose();
        }
    }
}
