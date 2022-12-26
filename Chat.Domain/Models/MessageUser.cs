using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Models
{
    public class MessageUser
    {
        public virtual User FromUser { get; set; }
        public virtual User ToUser { get; set; }
        public virtual Message Message { get; set; }
    }
}
