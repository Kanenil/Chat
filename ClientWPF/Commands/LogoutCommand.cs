using ClientWPF.Model;
using ClientWPF.Server;
using ClientWPF.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWPF.Commands
{
    public class LogoutCommand : CommandBase
    {
        private readonly AccountStore _accountStore;
        private readonly ServerConnection _server;

        public LogoutCommand(AccountStore accountStore, ServerConnection server)
        {
            _accountStore = accountStore;
            _server = server;
        }

        public override void Execute(object parameter)
        {
            _server.CloseConnection();
            _accountStore.Logout();
        }
    }
}
