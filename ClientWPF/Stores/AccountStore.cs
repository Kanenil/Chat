using BusinnesLogicLayer.DTO;
using ClientWPF.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWPF.Stores
{
    public class AccountStore
    {
        private UserDTO _currentAccount;
        public UserDTO CurrentAccount
        {
            get => _currentAccount;
            set
            {
                _currentAccount = value;
                CurrentAccountChanged?.Invoke();
            }
        }

        public bool IsLoggedIn => CurrentAccount != null;

        public event Action CurrentAccountChanged;

        public void Logout()
        {
            CurrentAccount = null;
        }
    }
}
