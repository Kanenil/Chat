using Chat.WPF.Commands.Base;
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

        public LoadUsersCommand(UserStore userStore)
        {
            _userStore = userStore;
        }

        public async override Task ExecuteAsync(object parameter)
        {
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
