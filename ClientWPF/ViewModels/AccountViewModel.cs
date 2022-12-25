using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
using ClientWPF.Commands;
using ClientWPF.Converters;
using ClientWPF.Model;
using ClientWPF.Server;
using ClientWPF.Services;
using ClientWPF.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClientWPF.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly IMessageUserService<MessageUserDTO> _messageUserService;
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
            set 
            {
                _selectedContact = value;

                if (value != null)
                {
                    _selectedLogin = _selectedContact.User.Login;
                    ConfigureMessages();
                }
                else
                    Messages.Clear();

                OnPropertyChanged(); 
            } 
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

        public AccountViewModel(AccountStore accountStore, INavigationService homeNavigationService, INavigationService settingsNavigationService, IService<UserDTO> service, IMessageUserService<MessageUserDTO> messageUserService, ServerConnection serverConnection)
        {
            Contacts = new ObservableCollection<ContactModel>();
            Messages = new ObservableCollection<MessageModel>();

            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
            _userService = service;
            _messageUserService = messageUserService;
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

            ConfigureChat();

            SendCommand = new SendMessageCommand(messageUserService, this, serverConnection);

            (SendCommand as SendMessageCommand).MessageSended += OnMessageSended;
            _accountStore.CurrentAccountChanged += OnCurrentAccountChanged;
        }

        private void ServerConnection_DataReceived()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await ConfigureChat(_serverConnection.LastMessage);
                SelectedContact = Contacts.First(u => u.User.Login == _selectedLogin);
            });
        }

        private async Task ConfigureChat()
        {
            Contacts.Clear();
            var contacts = await _userService.GetAllAsync();
            foreach (var contact in contacts)
            {
                if (contact.Id != _accountStore.CurrentAccount.Id)
                {
                    var messages = (await _messageUserService.Find(_accountStore.CurrentAccount.Id, contact.Id)).ToList();
                    messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));

                    Contacts.Add(new ContactModel()
                    {
                        User = contact,
                        LastMessage = messages.Count > 0 ? messages.Last().Message.Message : null
                    });
                }
            }
        }
        private async Task ConfigureMessages()
        {
            Messages.Clear();
            var messages = (await _messageUserService.Find(_accountStore.CurrentAccount.Id, SelectedContact.User.Id)).ToList();
            messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
            var color1 = String.Format("#{0:X6}", StaticRandom.Random(0x1000000));
            var color2 = String.Format("#{0:X6}", StaticRandom.Random(0x1000000));
            foreach (var message in messages)
            {
                Messages.Add(new MessageModel()
                {
                    User = message.FromUser,
                    ImageSource = message.FromUser.Photo,
                    Username = message.FromUser.Login,
                    IsNativeOrigin = false,
                    FirstMessage = Messages.Count == 0 ? true : Messages.Last().User.Id != message.FromUser.Id ? true : false,
                    Message = message.Message.Message,
                    Time = message.Message.Time,
                    UsernameColor = message.FromUser.Id == _accountStore.CurrentAccount.Id ? color1 : color2,
                });
            }
        }
        private async Task ConfigureChat(string login)
        {
            var contacts = Contacts.ToList();
            ContactModel contact = null;
            for (int i = 0; i < contacts.Count; i++)
            {
                if (contacts[i].User.Login == login)
                {
                    contact = contacts[i];
                    contacts.Remove(contact);
                    break;
                }
            }
            var messages = (await _messageUserService.Find(_accountStore.CurrentAccount.Id, contact.User.Id)).ToList();
            messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
            contact.LastMessage = messages.Last().Message.Message;
            contacts.Insert(0, contact);
            Contacts.Clear();
            foreach (var item in contacts)
                Contacts.Add(item);
            


            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    SelectedContact = null;

            //    var allMessages = _messageService.GetAll();
            //    ContactModel cont = new ContactModel();

            //    var login = _serverConnection.LastMessage;

            //    foreach (var contact in Contacts)
            //    {
            //        if (contact.User.Login == login)
            //        {
            //            cont = contact;
            //            break;
            //        }
            //    }

            //    var messages = allMessages.Where(m => (m.FromUser.Id == _accountStore.CurrentAccount.Id && m.ToUser.Id == cont.User.Id) || (m.ToUser.Id == _accountStore.CurrentAccount.Id && m.FromUser.Id == cont.User.Id)).ToList();
            //    messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
            //    var lastMessage = messages.Last();
            //    cont.Messages.Add(new MessageModel()
            //    {
            //        User = lastMessage.FromUser,
            //        ImageSource = lastMessage.FromUser.Photo,
            //        Username = lastMessage.FromUser.Login,
            //        IsNativeOrigin = false, 
            //        FirstMessage = cont.Messages.Count == 0 ? true : cont.Messages.Last().User.Id != lastMessage.FromUser.Id ? true : false,
            //        Message = lastMessage.Message.Message,
            //        Time = lastMessage.Message.Time
            //    });

            //    var contacts = new List<ContactModel>();
            //    foreach (var contact in Contacts)
            //    {
            //        if (contact.User.Login == login)
            //        {
            //            contacts.Add(cont);
            //            continue;
            //        }
            //        contacts.Add(contact);
            //    }

            //    contacts.Sort(0, contacts.Count,
            //        Comparer<ContactModel>.Create((a, b) =>
            //        {
            //            if (a.Messages.Count > 0 && b.Messages.Count > 0)
            //                return a.Messages.Last().Time < b.Messages.Last().Time ? 1 : a.Messages.Last().Time > b.Messages.Last().Time ? -1 : 0;
            //            return a.Messages.Count > 0 ? -1 : 1;
            //        }));

            //    Contacts = new ObservableCollection<ContactModel>();

            //    foreach (var item in contacts)
            //        Contacts.Add(item);

            //    SelectedContact = Contacts.Where(x => x.User.Login == _selectedLogin).First();
            //});
        }

        private async void OnMessageSended()
        {
            await ConfigureChat(_selectedLogin);
            SelectedContact = Contacts.First(u => u.User.Login == _selectedLogin);
        }
        private void OnCurrentAccountChanged()
        {
            OnPropertyChanged("User");
        }
        public override void Dispose()
        {
            _accountStore.CurrentAccountChanged -= OnCurrentAccountChanged;
            _serverConnection.DataReceived -= ServerConnection_DataReceived;
            (SendCommand as SendMessageCommand).MessageSended -= OnMessageSended;

            base.Dispose();
        }
    }
}
