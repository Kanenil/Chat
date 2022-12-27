using Chat.Domain.Models;
using Chat.WPF.Commands;
using Chat.WPF.Convertors;
using Chat.WPF.MVVM.Models;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Server;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly UserStore _userStore;
        public User User => _userStore.LoginedUser;
        public string Online => "Server Offline";

        private ObservableCollection<ContactModel> _contacts;
        public ObservableCollection<ContactModel> Contacts
        {
            get { return _contacts; }
            set { _contacts = value; OnPropertyChanged(); }
        }
        private string _selectedLogin;
        public string SelectedLogin
        {
            get { return _selectedLogin; }
            set { _selectedLogin = value; OnPropertyChanged(); }
        }

        private ContactModel _selectedContact;
        public ContactModel SelectedContact
        {
            get { return _selectedContact; }
            set 
            { 
                _selectedContact = value; 
                OnPropertyChanged();

                if (value == null || value.User.Login == _selectedLogin)
                    return;

                _selectedLogin = value.User.Login;
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
            set { _message = value; OnPropertyChanged(); }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }


        public HomeViewModel(UserStore userStore, 
                             INavigationService settingsNavigationService, 
                             ServerConnection serverConnection)
        {
            _userStore = userStore;

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
            Application.Current.Dispatcher.Invoke(() =>
            {
                int id = 0;
                Int32.TryParse(user, out id);

                if (SelectedContact == null)
                {
                    UsersLoaded();
                    return;
                }

                if (SelectedContact != null && SelectedContact.User.Id != id)
                {
                    UsersLoaded();
                    SelectedContact = Contacts.FirstOrDefault(x => x.User.Login == _selectedLogin);
                    return;
                }
                else if(SelectedContact != null)
                {
                    if (SelectedContact.User.Login != user && SelectedContact.User.Id != id)
                    {
                        UsersLoaded();
                        SelectedContact = Contacts.FirstOrDefault(x => x.User.Login == _selectedLogin);
                        return;
                    }
                }


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
                SelectedContact = Contacts.FirstOrDefault(x => x.User.Login == _selectedLogin);
            });
        }

        private void UsersLoaded()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                //IsLoading = true;
                Contacts.Clear();
                foreach (var user in _userStore.Users)
                    if (_userStore.LoginedUser.Id != user.Id)
                    {
                        var lastMessage = await _userStore.GetLastMessageFrom(_userStore.LoginedUser.Id, user.Id);
                        Contacts.Add(new ContactModel() { User = user, Online = _userStore.ConnectedUsers.Contains(user.Login), LastMessage = lastMessage != null ? lastMessage.Message.Info : "" });
                    }
                SelectedContact = Contacts.FirstOrDefault(x => x.User.Login == _selectedLogin);
                IsLoading = false;
            });
        }

        private void ConnectedUsersChanged()
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
                SelectedContact = Contacts.FirstOrDefault(x => x.User.Login == _selectedLogin);
            });
        }
    }
}
