using DataLayer.Data;
using DataLayer.Data.Entities;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class MessageUserRepository : IMessageUserRepository<MessageUserEntity>
    {
        private ChatContext _dataContext;
        public MessageUserRepository(ChatContext context)
        {
            _dataContext = context;
        }

        public async Task Create(MessageUserEntity item)
        {
            var userFrom = await _dataContext.Users.FindAsync(item.FromUser.Id);
            var userTo = await _dataContext.Users.FindAsync(item.ToUser.Id);
            var message = item.Message;
            message.User = await _dataContext.Users.FindAsync(message.User.Id);

            if (item != null)
                await _dataContext.MessageUsers.AddAsync(new MessageUserEntity()
                {
                    FromUser = userFrom,
                    ToUser = userTo,
                    Message= message
                });
        }

        public void Delete(MessageUserEntity id)
        {
            _dataContext.MessageUsers.Remove(id);
        }

        public async Task<IEnumerable<MessageUserEntity>> Find(int fromId, int toId)
        {
            var list = await _dataContext.MessageUsers.Where(u=>(u.UserFromId == fromId && u.UserToId == toId) || (u.UserFromId == toId && u.UserToId == fromId)).ToListAsync();
            foreach (var entity in list)
            {
                entity.FromUser = await _dataContext.Users.FindAsync(entity.UserFromId);
                entity.ToUser = await _dataContext.Users.FindAsync(entity.UserToId);
                entity.Message = await _dataContext.Messages.FindAsync(entity.MessageId);
            }
            list.Sort(0, list.Count, Comparer<MessageUserEntity>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
            return list;
        }

        public async Task<MessageUserEntity> FindLast(int fromId, int toId)
        {
            try
            {
                var messages = (List<MessageUserEntity>)await Find(fromId, toId);
                messages.Sort(0, messages.Count, Comparer<MessageUserEntity>.Create((a, b) => a.Message.Time > b.Message.Time ? 1 : a.Message.Time < b.Message.Time ? -1 : 0));
                var item = messages.Last();
                item.FromUser = await _dataContext.Users.FindAsync(item.UserFromId);
                item.ToUser = await _dataContext.Users.FindAsync(item.UserToId);
                item.Message = await _dataContext.Messages.FindAsync(item.MessageId);
                return item;
            }
            catch { return null; }
        }

        public async Task<IEnumerable<MessageUserEntity>> GetAll()
        {
            var list = await _dataContext.MessageUsers.ToListAsync();
            foreach (var entity in list)
            {
                entity.FromUser = await _dataContext.Users.FindAsync(entity.UserFromId);
                entity.ToUser = await _dataContext.Users.FindAsync(entity.UserToId);
                entity.Message = await _dataContext.Messages.FindAsync(entity.MessageId);
            }
            return list;
        }

        public async Task<IEnumerable<MessageUserEntity>> GetCount(int count)
        {
            var list = await _dataContext.MessageUsers.Take(count).ToListAsync();
            foreach (var entity in list)
            {
                entity.FromUser = await _dataContext.Users.FindAsync(entity.UserFromId);
                entity.ToUser = await _dataContext.Users.FindAsync(entity.UserToId);
                entity.Message = await _dataContext.Messages.FindAsync(entity.MessageId);
            }
            return list;
        }
    }
}
