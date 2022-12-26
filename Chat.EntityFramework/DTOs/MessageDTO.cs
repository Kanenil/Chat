using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework.DTOs
{
    [Table("tblMessage")]
    public class MessageDTO
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Message { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual UserDTO User { get; set; }
    }
}
