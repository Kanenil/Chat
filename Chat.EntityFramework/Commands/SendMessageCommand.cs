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
    public class SendMessageCommand : ISendMessageCommand
    {
        private readonly ChatDbContextFactory _contextFactory;

        public SendMessageCommand(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Execute(MessageUser user)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                MessageUserDTO messageUserDTO = new MessageUserDTO()
                {
                    FromUser = await context.Users.FindAsync(user.FromUser.Id),
                    ToUser= await context.Users.FindAsync(user.ToUser.Id),
                    Message = new MessageDTO()
                    {
                        Message = user.Message.Info,
                        Time = user.Message.Time,
                        User = await context.Users.FindAsync(user.Message.User.Id)
                    }
                };

                context.MessageUsers.Add(messageUserDTO);
                await context.SaveChangesAsync();
            }
        }
    }
}
