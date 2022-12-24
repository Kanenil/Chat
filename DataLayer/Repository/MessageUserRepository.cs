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
            if (item != null)
                await _dataContext.MessageUsers.AddAsync(item);
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
            var list = _dataContext.MessageUsers;
            foreach (var entity in list)
                _dataContext.Entry(entity).State = EntityState.Detached;
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
