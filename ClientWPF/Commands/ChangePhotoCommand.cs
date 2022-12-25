using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using ClientWPF.Model;
using ClientWPF.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Commands
{
    public class ChangePhotoCommand : AsyncCommandBase
    {
        private readonly IService<UserDTO> _service;
        private readonly AccountStore _accountStore;

        public ChangePhotoCommand(IService<UserDTO> service, AccountStore accountStore)
        {
            _service = service;
            _accountStore = accountStore;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            OpenFileDialog res = new OpenFileDialog();
            res.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            res.Multiselect = false;
            if (res.ShowDialog() != false)
            {
                var path = PhotoSaver.UploadImage(File.ReadAllBytes(res.FileName));
                var user = _accountStore.CurrentAccount;
                user.Photo = path;
                _accountStore.CurrentAccount = user;
                await _service.UpdateDTO(user);
            }
        }
    }
}
