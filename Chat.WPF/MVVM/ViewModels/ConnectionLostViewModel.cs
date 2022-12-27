using Chat.WPF.Commands;
using Chat.WPF.MVVM.ViewModels.Base;
using Chat.WPF.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.WPF.MVVM.ViewModels
{
    public class ConnectionLostViewModel : ViewModelBase
    {
        public ICommand CancelCommand { get; }
        public ConnectionLostViewModel(INavigationService navigationService)
        {
            CancelCommand = new NavigateCommand(navigationService);
        }
    }
}
