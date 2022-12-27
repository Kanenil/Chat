using Chat.Domain.Models;
using Chat.WPF.Commands.Base;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class LoadMessagesCommand : AsyncCommandBase
    {
        private readonly UserStore _userStore;
        private readonly HomeViewModel _viewModel;

        public LoadMessagesCommand(UserStore userStore, HomeViewModel viewModel)
        {
            _userStore = userStore;
            _viewModel = viewModel;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            if (_viewModel.SelectedContact != null)
                await _userStore.LoadMessages(_viewModel.SelectedContact.User.Id);
        }
    }
}
