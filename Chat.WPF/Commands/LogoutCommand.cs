using Chat.WPF.Commands.Base;
using Chat.WPF.Server;
using Chat.WPF.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Commands
{
    public class LogoutCommand : CommandBase
    {
        private readonly INavigationService _navigationService;
        private readonly ServerConnection _serverConnection;
        public LogoutCommand(INavigationService logoutNavigationService, ServerConnection serverConnection)
        {
            _navigationService = logoutNavigationService;
            _serverConnection = serverConnection;
        }

        public override void Execute(object parameter)
        {
            if (_serverConnection.IsConnected)
                _serverConnection.CloseConnection(false);
            _navigationService.Navigate();
        }
    }
}
