using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string Login { get; set; }
        [Required, StringLength(100)]
        public string Email { get; set; }
        [Required, StringLength(20)]
        public string Password { get; set; }
        [StringLength(255)]
        public string Photo { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
