using Chat.Domain.Commands;
using Chat.EntityFramework.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework.Commands
{
    public class DeleteAllUserMessageCommand : IDeleteAllUserMessageCommand
    {
        private readonly ChatDbContextFactory _contextFactory;

        public DeleteAllUserMessageCommand(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Execute(int id)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                var messages = (await context.MessageUsers.ToListAsync()).Where(u=>u.UserFromId== id || u.UserToId == id).ToList();
                context.MessageUsers.RemoveRange(messages);
                await context.SaveChangesAsync();
            }
        }
    }
}
