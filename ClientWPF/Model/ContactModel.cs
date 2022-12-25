﻿using BusinnesLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Model
{
    public class ContactModel
    {
        public UserDTO User { get; set; }
        public string LastMessage { get; set; }
        public bool Online { get; set; }
    }
}
