using Chat.WPF.Commands.Base;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class DelayNavigationCommand : AsyncCommandBase
    {
        private readonly INavigationService _navigationService;

        public DelayNavigationCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            await Task.Delay(500);
            _navigationService.Navigate();
        }
    }
}
