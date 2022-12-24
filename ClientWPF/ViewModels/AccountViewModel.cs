using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Commands;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly AccountStore _accountStore;
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

        private ContactModel _selectedContact;
        public ContactModel SelectedContact 
        { 
            get => _selectedContact; 
            set { _selectedContact = value; Messages = _selectedContact.Messages; OnPropertyChanged(); } 
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand SendCommand { get; }

        public AccountViewModel(AccountStore accountStore, INavigationService homeNavigationService, INavigationService settingsNavigationService, IService<UserDTO> service, IService<MessageUserDTO> messageUserService)
        {
            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);

            var allMessages = messageUserService.GetAll();

            Contacts = new ObservableCollection<ContactModel>();
            foreach (var item in service.GetAll())
            {
                if (item.Id != _accountStore.CurrentAccount.Id)
                {
                    var messages = allMessages.Where(m => (m.FromUser.Id == _accountStore.CurrentAccount.Id && m.ToUser.Id == item.Id) || (m.ToUser.Id == _accountStore.CurrentAccount.Id && m.FromUser.Id == item.Id)).ToList();
                    ObservableCollection<MessageModel> messages1 = new ObservableCollection<MessageModel>();
                    foreach (var message in messages)
                    {
                        messages1.Add(new MessageModel()
                        {
                            User = message.FromUser,
                            ImageSource = message.FromUser.Photo,
                            Username = message.FromUser.Login,
                            IsNativeOrigin = message.ToUser.Id == _accountStore.CurrentAccount.Id?false:true,
                            FirstMessage = messages1.Count == 0 ? true : messages.Last().ToUser.Id == _accountStore.CurrentAccount.Id? false: true,
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

            SendCommand = new SendMessageCommand(messageUserService, this);

            _accountStore.CurrentAccountChanged += OnCurrentAccountChanged;
        }

        private void OnCurrentAccountChanged()
        {
            OnPropertyChanged("User");
        }

        public override void Dispose()
        {
            _accountStore.CurrentAccountChanged -= OnCurrentAccountChanged;

            base.Dispose();
        }
    }
}
