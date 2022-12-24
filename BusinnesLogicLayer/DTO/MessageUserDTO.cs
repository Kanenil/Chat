using DataLayer.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.DTO
{
    public class MessageUserDTO
    {
        public int UserFromId { get; set; }
        public virtual UserDTO FromUser { get; set; }
        public int UserToId { get; set; }
        public virtual UserDTO ToUser { get; set; }
        public int MessageId { get; set; }
        public virtual MessageDTO Message { get; set; }
    }
}
