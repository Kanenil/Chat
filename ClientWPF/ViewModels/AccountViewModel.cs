﻿using BusinnesLogicLayer.DTO;
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
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private System.Timers.Timer connectionTimer;
        private System.Timers.Timer checkConnection;

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
        private string _online;
        public string Online { get => _online; set { _online = value; OnPropertyChanged(); } }
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand SendCommand { get; }

        public Task Initialization { get; }

        public AccountViewModel(AccountStore accountStore, INavigationService homeNavigationService, INavigationService settingsNavigationService, IService<UserDTO> service, IMessageUserService<MessageUserDTO> messageUserService, ServerConnection serverConnection)
        {
            Contacts = new ObservableCollection<ContactModel>();
            Messages = new ObservableCollection<MessageModel>();

            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
            _userService = service;
            _messageUserService = messageUserService;
            _serverConnection = serverConnection;
            Online = "Server Offline";
            SendCommand = new SendMessageCommand(messageUserService, this, serverConnection);
            (SendCommand as SendMessageCommand).MessageSended += OnMessageSended;
            _accountStore.CurrentAccountChanged += OnCurrentAccountChanged;

            Initialization = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await Task.WhenAll(ConfigureChat());

            if (_serverConnection.IsConnected)
            {
                Online = "Server Online";
                return;
            }

            try
            {
                await _serverConnection.Connect();

                _serverConnection.DataReceived += ServerConnection_DataReceived;
                _serverConnection.ConnectedUsersChanged += ServerConnection_ConnectedUsersChanged;

                await _serverConnection.Rename(_accountStore.CurrentAccount.Login);
                await _serverConnection.GetAllConnectedUsers(_accountStore.CurrentAccount.Login);
            }
            catch
            {
                connectionTimer = new System.Timers.Timer();

                connectionTimer.Interval = 5000;
                connectionTimer.Elapsed += OnTimedEvent;
                connectionTimer.AutoReset = true;
                connectionTimer.Enabled = true;


            }
        }

        private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                await _serverConnection.Connect();

                _serverConnection.DataReceived += ServerConnection_DataReceived;
                _serverConnection.ConnectedUsersChanged += ServerConnection_ConnectedUsersChanged;

                await _serverConnection.Rename(_accountStore.CurrentAccount.Login);
                await _serverConnection.GetAllConnectedUsers(_accountStore.CurrentAccount.Login);

                Online = "Server Online";

                checkConnection = new System.Timers.Timer();

                checkConnection.Interval = 5000;
                checkConnection.Elapsed += OnConnectionTimedEvent;
                checkConnection.AutoReset = true;
                checkConnection.Enabled = true;

                connectionTimer.Stop();
            }
            catch 
            {

            }
        }

        private void OnConnectionTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (!_serverConnection.IsConnected)
            {
                checkConnection.Stop();
                Online = "Server Offline";
                _serverConnection.ConnectedUsers.Clear();
                connectionTimer.Start();

                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await ConfigureChat();
                });
            }
        }

        private void ServerConnection_ConnectedUsersChanged()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                //Працює, але мінус того що він обновляє всю колекцію
                await ConfigureChat();
                SelectedContact = Contacts.First(u => u.User.Login == _selectedLogin);
            });
        }

        private void ServerConnection_DataReceived()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (_serverConnection.LastMessage.Split(' ').Contains("connected"))
                {
                    await _serverConnection.GetAllConnectedUsers(_accountStore.CurrentAccount.Login);
                }
                else if(_serverConnection.LastMessage.Split(' ').Contains("disconnected"))
                {
                    await _serverConnection.GetAllConnectedUsers(_accountStore.CurrentAccount.Login);
                }
                else
                {
                    await ConfigureChat(_serverConnection.LastMessage);
                    SelectedContact = Contacts.First(u => u.User.Login == _selectedLogin);
                }
            });
        }

        private async Task ConfigureChat()
        {
            Contacts.Clear();
            var contacts = await _userService.GetAllAsync();
            foreach (var contact in contacts)
            {
                if (_accountStore.CurrentAccount == null) return;
                if (contact.Id != _accountStore.CurrentAccount.Id)
                {
                    try
                    {
                        Contacts.First(x => x.User.Id == contact.Id);
                    }
                    catch 
                    {
                        var message = await _messageUserService.FindLast(_accountStore.CurrentAccount.Id, contact.Id);

                        Contacts.Add(new ContactModel()
                        {
                            User = contact,
                            LastMessage = message != null ? message.Message.Message : null,
                            Online = _serverConnection.ConnectedUsers != null ? _serverConnection.ConnectedUsers.Contains(contact.Login) : false
                        });
                    }
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

            if (_accountStore.CurrentAccount.Login == login) return;

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
            if (contact == null)
            {
                await Task.Delay(1000);
                await ConfigureChat(login);
            }

            var messages = await _messageUserService.Find(_accountStore.CurrentAccount.Id, contact.User.Id);
            contact.LastMessage = messages.Last().Message.Message;
            contact.Online = _serverConnection.ConnectedUsers.Contains(contact.User.Login);
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
            checkConnection.Dispose();
            connectionTimer.Dispose();

            base.Dispose();
        }
    }
}
