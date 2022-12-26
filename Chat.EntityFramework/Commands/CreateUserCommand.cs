using Chat.Domain.Commands;
using Chat.Domain.Models;
using Chat.EntityFramework.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework.Commands
{
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly ChatDbContextFactory _contextFactory;

        public CreateUserCommand(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task Execute(User user)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                UserDTO userDTO = new UserDTO()
                {
                    Login = user.Login,
                    Email= user.Email,
                    Password= user.Password,
                    Photo = user.Photo,
                    EmailConfirmed= user.EmailConfirmed
                };

                context.Users.Add(userDTO);
                await context.SaveChangesAsync();
            }
        }
    }
}
