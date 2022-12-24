using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Commands
{
    public class ChangePhotoCommand : CommandBase
    {
        private readonly IService<UserDTO> _service;
        private readonly AccountStore _accountStore;

        public ChangePhotoCommand(IService<UserDTO> service, AccountStore accountStore)
        {
            _service = service;
            _accountStore = accountStore;
        }

        public override void Execute(object parameter)
        {
            OpenFileDialog res = new OpenFileDialog();
            res.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            res.Multiselect = false;
            if (res.ShowDialog() != false)
            {
                var user = _accountStore.CurrentAccount;
                user.Photo = res.FileName;
                _accountStore.CurrentAccount = user;
            }
        }
    }
}
