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
                    var message = await _messageUserService.FindLast(_accountStore.CurrentAccount.Id, contact.Id);

                    Contacts.Add(new ContactModel()
                    {
                        User = contact,
                        LastMessage = message != null ? message.Message.Message : null
                    });
                }
            }
        }
        private async Task ConfigureMessages()
        {
            Messages.Clear();
            var messages = (await _messageUserService.Find(_accountStore.CurrentAccount.Id, SelectedContact.User.Id)).ToList();
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
            contact.LastMessage = messages.Last().Message.Message;
            contacts.Insert(0, contact);
            Contacts.Clear();
            foreach (var item in contacts)
                Contacts.Add(item);
            
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
