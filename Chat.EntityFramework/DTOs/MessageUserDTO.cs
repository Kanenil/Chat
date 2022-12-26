using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework.DTOs
{
    [Table("tblMessageUsers")]
    public class MessageUserDTO
    {
        [ForeignKey("FromUser")]
        public int UserFromId { get; set; }
        public virtual UserDTO FromUser { get; set; }
        [ForeignKey("ToUser")]
        public int UserToId { get; set; }
        public virtual UserDTO ToUser { get; set; }
        [ForeignKey("Message")]
        public int MessageId { get; set; }
        public virtual MessageDTO Message { get; set; }
    }
}
