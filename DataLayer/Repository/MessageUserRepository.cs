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
    public class MessageUserRepository : IRepository<MessageUserEntity>
    {
        private ChatContext _dataContext;
        public MessageUserRepository(ChatContext context)
        {
            _dataContext = context;
        }

        public async Task Create(MessageUserEntity item)
        {
            var userFrom = _dataContext.Users.Find(item.FromUser.Id);
            var userTo = _dataContext.Users.Find(item.ToUser.Id);
            var message = item.Message;
            message.User = _dataContext.Users.Find(message.User.Id);

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

        public async Task<MessageUserEntity> Find(MessageUserEntity id)
        {
            var messageUsers = await _dataContext.MessageUsers.FindAsync(id);
            if (messageUsers != null)
                return messageUsers;
            throw new InvalidOperationException();
        }

        public IEnumerable<MessageUserEntity> GetAll()
        {
            var list = _dataContext.MessageUsers.ToList();
            foreach (var entity in list)
            {
                entity.FromUser = _dataContext.Users.Find(entity.UserFromId);
                entity.ToUser = _dataContext.Users.Find(entity.UserToId);
                entity.Message = _dataContext.Messages.Find(entity.MessageId); 
            }

            return list;
        }

        public int GetId(MessageUserEntity item)
        {
            throw new InvalidOperationException();
        }

        public void Update(MessageUserEntity item)
        {
            throw new InvalidOperationException();
        }
    }
}
