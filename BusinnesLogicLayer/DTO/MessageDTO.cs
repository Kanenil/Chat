using DataLayer.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Message { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public int? UserId { get; set; }
        public virtual UserDTO User { get; set; }
    }
}
