using Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.MVVM.Models
{
    public class MessageModel
    {
        public User User { get; set; }
        public string Username { get; set; }
        public string UsernameColor { get; set; }
        public string ImageSource { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public bool IsNativeOrigin { get; set; }
        public bool? FirstMessage { get; set; }
    }
}
