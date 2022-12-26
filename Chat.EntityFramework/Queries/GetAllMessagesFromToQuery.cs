using Chat.Domain.Models;
using Chat.Domain.Queries;
using Chat.EntityFramework.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework.Queries
{
    public class GetAllMessagesFromToQuery : IGetAllMessagesFromToQuery
    {
        private readonly ChatDbContextFactory _contextFactory;

        public GetAllMessagesFromToQuery(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<MessageUser>> Execute(int fromId, int toId)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                List<MessageUserDTO> messages = await context.MessageUsers
                                                             .Include(u => u.ToUser)
                                                             .Include(u => u.FromUser)
                                                             .Include(u => u.Message)
                                                             .Where(u => (u.UserFromId == fromId && u.UserToId == toId) || (u.UserFromId == toId && u.UserToId == fromId))
                                                             .ToListAsync();
                if (messages.Count > 1)
                    messages.Sort(0, messages.Count, Comparer<MessageUserDTO>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));

                return messages.Select(u => new MessageUser()
                {
                    FromUser = new User(u.FromUser.Id, u.FromUser.Login, u.FromUser.Email, u.FromUser.Password, u.FromUser.Photo, u.FromUser.EmailConfirmed),
                    ToUser = new User(u.ToUser.Id, u.ToUser.Login, u.ToUser.Email, u.ToUser.Password, u.ToUser.Photo, u.ToUser.EmailConfirmed),
                    Message = new Message(u.Message.Id, u.Message.Message, u.Message.Time, new User(u.Message.User.Id, u.Message.User.Login, u.Message.User.Email, u.Message.User.Password, u.Message.User.Photo, u.Message.User.EmailConfirmed))
                });
            }
        }
    }
}
