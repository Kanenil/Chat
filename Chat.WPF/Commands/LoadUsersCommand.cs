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
    public class LoadUsersCommand : AsyncCommandBase
    {
        private readonly UserStore _userStore;
        private readonly HomeViewModel _viewModel;


        public LoadUsersCommand(HomeViewModel viewModel,UserStore userStore)
        {
            _userStore = userStore;
            _viewModel = viewModel;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _viewModel.IsLoading = true;

            try
            {
                await _userStore.Load();
            }
            catch (Exception)
            {

            }
            finally
            {
                
            }
        }
    }
}
