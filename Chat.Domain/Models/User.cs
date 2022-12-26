using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Photo { get; set; }
        public bool EmailConfirmed { get; set; }

        public User(int id, string login, string email, string password, string? photo, bool emailConfirmed)
        {
            Id = id;
            Login = login;
            Email = email;
            Password = password;
            Photo = photo;
            EmailConfirmed = emailConfirmed;
        }
    }
}
