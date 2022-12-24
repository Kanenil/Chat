using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Data.Entities
{
    [Table("tblMessageUsers")]
    public class MessageUserEntity
    {
        [ForeignKey("FromUser")]
        public int UserFromId { get; set; }
        public virtual UserEntity FromUser { get; set; }
        [ForeignKey("ToUser")]
        public int UserToId { get; set; }
        public virtual UserEntity ToUser { get; set; }
        [ForeignKey("Message")]
        public int MessageId { get; set; }
        public virtual MessageEntity Message { get; set; }
    }
}
