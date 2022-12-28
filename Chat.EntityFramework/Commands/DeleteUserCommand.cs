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
    public class DeleteUserCommand : IDeleteUserCommand
    {
        private readonly ChatDbContextFactory _contextFactory;

        public DeleteUserCommand(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task Execute(int id)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                UserDTO userDTO = new UserDTO()
                {
                    Id = id
                };

                context.Users.Remove(userDTO);
                await context.SaveChangesAsync();
            }
        }
    }
}
