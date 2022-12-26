using Chat.WPF.Commands.Base;
using Chat.WPF.Convertors;
using Chat.WPF.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class ChangePhotoCommand : AsyncCommandBase
    {
        private readonly UserStore _userStore;

        public ChangePhotoCommand(UserStore userStore)
        {
            _userStore = userStore;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            OpenFileDialog res = new OpenFileDialog();
            res.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            res.Multiselect = false;
            if (res.ShowDialog() != false)
            {
                var path = PhotoSaver.UploadImage(File.ReadAllBytes(res.FileName));
                var user = _userStore.LoginedUser;
                user.Photo = path;
                _userStore.LoginedUser = user;
                await _userStore.UpdateUser(user);
            }
        }
    }
}
