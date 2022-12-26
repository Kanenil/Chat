using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Info { get; set; }
        public DateTime Time { get; set; }
        public virtual User User { get; set; }

        public Message(int id, string message, DateTime time, User user)
        {
            Id = id;
            Info = message;
            Time = time;
            User = user;
        }
    }
}
