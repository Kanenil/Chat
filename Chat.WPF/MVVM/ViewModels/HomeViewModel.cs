using Chat.Domain.Models;
using Chat.WPF.Commands;
using Chat.WPF.Convertors;
using Chat.WPF.MVVM.Models;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Server;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly UserStore _userStore;
        private readonly ServerConnection _serverConnection;
        public User User => _userStore.LoginedUser;
        public string Online => _serverConnection.IsConnected ? "Server Online" : "Server Offline";

        private ObservableCollection<ContactModel> _contacts;
        public ObservableCollection<ContactModel> Contacts
        {
            get
            {
                if (_contacts != null)
                    CollectionViewSource.GetDefaultView(_contacts).Refresh();
                return _contacts ?? (_contacts = new ObservableCollection<ContactModel>());
            }
            set
            {
                if (_contacts == value)
                    return;

                _contacts = value;
                OnPropertyChanged("Contacts");
            }
        }
 

        private ContactModel _selectedContact;
        private ContactModel _prevSelectedContact;
        public ContactModel SelectedContact
        {
            get { return _selectedContact; }
            set 
            {
                _prevSelectedContact = _selectedContact;
                _selectedContact = value; 
                OnPropertyChanged("SelectedContact");

                if (value == null)
                    return;

                LoadMessages.Execute(null);
            }
        }

        private ObservableCollection<MessageModel> _messages;
        public ObservableCollection<MessageModel> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged("IsLoading"); }
        }

        private bool _isLoadingMessages;

        public bool IsLoadingMessages
        {
            get { return _isLoadingMessages; }
            set { _isLoadingMessages = value; OnPropertyChanged("IsLoadingMessages"); }
        }

        private bool _isLoadingUsers;

        public bool IsLoadingUsers
        {
            get { return _isLoadingUsers; }
            set { _isLoadingUsers = value; OnPropertyChanged("IsLoadingUsers"); }
        }

        public HomeViewModel(UserStore userStore, 
                             INavigationService settingsNavigationService, 
                             ServerConnection serverConnection)
        {
            _userStore = userStore;
            _serverConnection = serverConnection;

            _userStore.UsersLoaded += UsersLoaded;
            _userStore.MessagesLoaded += MessagesLoaded;
            _userStore.ConnectedUsersChanged += ConnectedUsersChanged;
            _userStore.LoginedUserChanged += LoginedUserChanged;

            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
            LoadContactsCommand = new LoadUsersCommand(this ,userStore);
            SendCommand = new SendMessageCommand(this, userStore, serverConnection);
            LoadMessages = new LoadMessagesCommand(userStore, this);

            (SendCommand as SendMessageCommand).MessageSended += MessageSended;

            Contacts = new ObservableCollection<ContactModel>();
            Messages = new ObservableCollection<MessageModel>();
        }

        private void LoginedUserChanged()
        {
            LoadContactsCommand.Execute(null);
        }
        private void MessageSended()
        {
            LoadMessages.Execute(null);
        }

        public ICommand LoadContactsCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand SendCommand { get; }
        private ICommand LoadMessages { get; }

        public static HomeViewModel LoadViewModel(UserStore userStore, 
                                                  INavigationService settingsNavigationService, 
                                                  ServerConnection serverConnection)
        {
            HomeViewModel viewModel = new HomeViewModel(userStore, settingsNavigationService, serverConnection);

            viewModel.LoadContactsCommand.Execute(null);

            return viewModel;
        }
        private void MessagesLoaded(string user)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                IsLoadingMessages = true;

                bool isUserName = false;

                MessageUser? lastMessage;

                int id = 0;
                if(Int32.TryParse(user, out id))
                {
                    var contact = Contacts.FirstOrDefault(u=>u.User.Id == id);
                    var index = Contacts.IndexOf(contact);
                    lastMessage = await _userStore.GetLastMessageFrom(_userStore.LoginedUser.Id, contact.User.Id);
                    Contacts[index].LastMessage = lastMessage != null ? lastMessage.Message.Info : null;

                    if (SelectedContact == null || SelectedContact.User.Id != id)
                    {
                        IsLoadingMessages = false;
                        return;
                    }
                }
                else
                {
                    var contact = Contacts.FirstOrDefault(u => u.User.Login == user);
                    var index = Contacts.IndexOf(contact);
                    lastMessage = await _userStore.GetLastMessageFrom(_userStore.LoginedUser.Id, contact.User.Id);
                    Contacts[index].LastMessage = lastMessage != null ? lastMessage.Message.Info : null;

                    isUserName = true;
                    
                    if (SelectedContact == null || SelectedContact.User.Login != user)
                    {
                        IsLoadingMessages = false;
                        return;
                    }
                }



                if ((Messages.Count == 0 && _prevSelectedContact == null) || !(_prevSelectedContact == null || _prevSelectedContact.User.Id == _selectedContact.User.Id))
                {

                    Messages.Clear();
                    var color1 = String.Format("#{0:X6}", StaticRandom.Random(0x1000000));
                    var color2 = String.Format("#{0:X6}", StaticRandom.Random(0x1000000));
                    foreach (var message in _userStore.MessageUsers)
                    {
                        Messages.Add(new MessageModel()
                        {
                            User = message.FromUser,
                            ImageSource = message.FromUser.Photo,
                            Username = message.FromUser.Login,
                            IsNativeOrigin = false,
                            FirstMessage = Messages.Count == 0 ? true : Messages.Last().User.Id != message.FromUser.Id ? true : false,
                            Message = message.Message.Info,
                            Time = message.Message.Time,
                            UsernameColor = message.FromUser.Id == _userStore.LoginedUser.Id ? color1 : color2,
                        });
                    }
                }
                else
                {
                    MessageModel? lastMessageFrom = isUserName ? Messages.Where(m => m.Username == user).LastOrDefault() : Messages.Where(m => m.User.Id == id).LastOrDefault();

                    if (lastMessage.FromUser.Id != _userStore.LoginedUser.Id)
                    {
                        if (lastMessageFrom != null)
                        {
                            if (!(lastMessage.Message.Info == Messages.Last().Message || lastMessage.Message.Time == Messages.Last().Time))
                            {
                                Messages.Add(new MessageModel()
                                {
                                    User = lastMessage.FromUser,
                                    ImageSource = lastMessage.FromUser.Photo,
                                    Username = lastMessage.FromUser.Login,
                                    IsNativeOrigin = false,
                                    FirstMessage = Messages.Count == 0 ? true : Messages.Last().User.Id != lastMessage.FromUser.Id ? true : false,
                                    Message = lastMessage.Message.Info,
                                    Time = lastMessage.Message.Time,
                                    UsernameColor = lastMessageFrom.UsernameColor
                                });
                            }

                        }
                        else
                        {
                            Messages.Add(new MessageModel()
                            {
                                User = lastMessage.FromUser,
                                ImageSource = lastMessage.FromUser.Photo,
                                Username = lastMessage.FromUser.Login,
                                IsNativeOrigin = false,
                                FirstMessage = Messages.Count == 0 ? true : Messages.Last().User.Id != lastMessage.FromUser.Id ? true : false,
                                Message = lastMessage.Message.Info,
                                Time = lastMessage.Message.Time,
                                UsernameColor = String.Format("#{0:X6}", StaticRandom.Random(0x1000000))
                            });
                        }
                    }
                    else
                    {
                        lastMessageFrom = Messages.Where(m => m.Username == _userStore.LoginedUser.Login).LastOrDefault();
                        if (lastMessageFrom != null)
                        {
                            if (!(lastMessage.Message.Info == Messages.Last().Message || lastMessage.Message.Time == Messages.Last().Time))
                            {
                                Messages.Add(new MessageModel()
                                {
                                    User = lastMessage.FromUser,
                                    ImageSource = lastMessage.FromUser.Photo,
                                    Username = lastMessage.FromUser.Login,
                                    IsNativeOrigin = false,
                                    FirstMessage = Messages.Count == 0 ? true : Messages.Last().User.Id != lastMessage.FromUser.Id ? true : false,
                                    Message = lastMessage.Message.Info,
                                    Time = lastMessage.Message.Time,
                                    UsernameColor = lastMessageFrom.UsernameColor
                                });
                            }
                        }
                        else
                        {
                            Messages.Add(new MessageModel()
                            {
                                User = lastMessage.FromUser,
                                ImageSource = lastMessage.FromUser.Photo,
                                Username = lastMessage.FromUser.Login,
                                IsNativeOrigin = false,
                                FirstMessage = Messages.Count == 0 ? true : Messages.Last().User.Id != lastMessage.FromUser.Id ? true : false,
                                Message = lastMessage.Message.Info,
                                Time = lastMessage.Message.Time,
                                UsernameColor = String.Format("#{0:X6}", StaticRandom.Random(0x1000000))
                    });
                        }

                    }
                }
                IsLoadingMessages = false;
            });
        }

        private void UsersLoaded()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                Contacts.Clear();
                foreach (var user in _userStore.Users)
                    if (_userStore.LoginedUser.Id != user.Id)
                    {
                        var lastMessage = await _userStore.GetLastMessageFrom(_userStore.LoginedUser.Id, user.Id);
                        Contacts.Add(new ContactModel() { User = user, Online = _userStore.ConnectedUsers.Contains(user.Login), LastMessage = lastMessage != null ? lastMessage.Message.Info : "" });
                    }
                IsLoading = false;
            });
        }

        private void ConnectedUsersChanged()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                IsLoadingUsers = true;

                if (Contacts.Count == 0)
                {
                    foreach (var user in _userStore.Users)
                        if (_userStore.LoginedUser.Id != user.Id)
                        {
                            var lastMessage = await _userStore.GetLastMessageFrom(_userStore.LoginedUser.Id, user.Id);
                            Contacts.Add(new ContactModel() { User = user, Online = _userStore.ConnectedUsers.Contains(user.Login), LastMessage = lastMessage != null ? lastMessage.Message.Info : "" });
                        }
                }

                string newContact = "";
                foreach (var connectedUser in _userStore.ConnectedUsers)
                {
                    string isNew = "";
                    foreach (var contact in Contacts)
                    {
                        isNew = connectedUser;
                        if (connectedUser == contact.User.Login || connectedUser == _userStore.LoginedUser.Login)
                        {
                            isNew = "";
                            break;
                        }
                    }
                    if (isNew != "")
                    {
                        newContact = isNew;
                        break;
                    }
                }

                if (newContact != "")
                {
                    var user = await _userStore.GetUserByLoginOrEmail(newContact);
                    var lastMessage = await _userStore.GetLastMessageFrom(_userStore.LoginedUser.Id, user.Id);
                    Contacts.Add(new ContactModel() { User = user, Online = _userStore.ConnectedUsers.Contains(user.Login), LastMessage = lastMessage != null ? lastMessage.Message.Info : "" });
                }

                for (int i = 0; i < Contacts.Count; i++)
                    Contacts[i].Online = false;

                for (int i = 0; i < Contacts.Count; i++)
                {
                    foreach (var connectedUser in _userStore.ConnectedUsers)
                    {
                        if (Contacts[i].User.Login != connectedUser)
                            continue;
                        
                        Contacts[i].Online = true;
                    }
                }

                //Contacts.OrderBy(x => !x.Online);

                IsLoadingUsers = false;
                IsLoading = false;
            });
        }
    }
}
