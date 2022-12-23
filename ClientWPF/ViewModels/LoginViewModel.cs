using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
using ClientWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.ViewModels
{
    public class LoginViewModel : ViewModel
    {
        public readonly IService<UserDTO> _service;
        public LoginViewModel(IService<UserDTO> service)
        {
            _service = service;
            var users = _service.GetAll();
        }
    }
}
