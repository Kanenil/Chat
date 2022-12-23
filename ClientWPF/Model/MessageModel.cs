﻿using BusinnesLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Model
{
    public class MessageModel
    {
        public UserDTO User { get; set; }
        //public string Username { get; set; }
        public string UsernameColor => String.Format("#{0:X6}", StaticRandom.Random(0x1000000));
        //public string ImageSource { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public bool IsNativeOrigin { get; set; }
        public bool? FirstMessage { get; set; }
    }
}
