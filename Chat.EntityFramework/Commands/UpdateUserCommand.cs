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
    public class UpdateUserCommand : IUpdateUserCommand
    {
        private readonly ChatDbContextFactory _contextFactory;

        public UpdateUserCommand(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Execute(User user)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                UserDTO userDto = new UserDTO()
                {
                    Id = user.Id,
                    Login = user.Login,
                    Email= user.Email,
                    EmailConfirmed= user.EmailConfirmed,
                    Password= user.Password,
                    Photo = user.Photo
                };

                context.Users.Update(userDto);
                await context.SaveChangesAsync();
            }
        }
    }
}
