using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Commands;
using ClientWPF.Model;
using ClientWPF.Services;
using ClientWPF.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ClientWPF.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly AccountStore _accountStore;
        private readonly IService<MessageUserDTO> _messageUser;
        public UserDTO User => _accountStore.CurrentAccount;
        public ObservableCollection<ContactModel> Contacts { get; set; }
        private ContactModel _selectedContact;
        public ContactModel SelectedContact { get => _selectedContact; set { _selectedContact = value; OnPropertyChanged(); } }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand SendMessageCommand { get; }

        public AccountViewModel(AccountStore accountStore, INavigationService homeNavigationService, INavigationService settingsNavigationService, IService<UserDTO> service, IService<MessageUserDTO> messageUserService)
        {
            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);

            _messageUser = messageUserService;

            Contacts = new ObservableCollection<ContactModel>();
            foreach (var item in service.GetAll())
            {
                if (item.Id != _accountStore.CurrentAccount.Id)
                {
                    Contacts.Add(new ContactModel()
                    {
                        User = item
                    });
                }
            }

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
