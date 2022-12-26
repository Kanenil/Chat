using Chat.Domain.Models;
using Chat.Domain.Queries;
using Chat.EntityFramework.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework.Queries
{
    public class GetLastMessageFromToQuery : IGetLastMessageFromToQuery
    {
        private readonly ChatDbContextFactory _contextFactory;

        public GetLastMessageFromToQuery(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<MessageUser> Execute(int fromId, int toId)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                var messages = await context.MessageUsers
                                            .Include(u=>u.ToUser)
                                            .Include(u=>u.FromUser)
                                            .Include(u=>u.Message)
                                            .Where(u => (u.UserFromId == fromId && u.UserToId == toId) || (u.UserFromId == toId && u.UserToId == fromId))
                                            .ToListAsync();
                messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
                var lastMessage = messages.LastOrDefault();
                if (lastMessage != null)
                {
                    return new MessageUser()
                    {
                        FromUser = new User(lastMessage.FromUser.Id, lastMessage.FromUser.Login,lastMessage.FromUser.Email,lastMessage.FromUser.Password,lastMessage.FromUser.Photo,lastMessage.FromUser.EmailConfirmed),
                        ToUser = new User(lastMessage.ToUser.Id, lastMessage.ToUser.Login,lastMessage.ToUser.Email,lastMessage.ToUser.Password,lastMessage.ToUser.Photo,lastMessage.ToUser.EmailConfirmed),
                        Message = new Message(lastMessage.Message.Id,lastMessage.Message.Message, lastMessage.Message.Time, new User(lastMessage.Message.User.Id, lastMessage.Message.User.Login, lastMessage.Message.User.Email, lastMessage.Message.User.Password, lastMessage.Message.User.Photo, lastMessage.Message.User.EmailConfirmed))
                    };
                }
                return null;
            }
        }
    }
}
