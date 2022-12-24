using DataLayer.Data;
using DataLayer.Data.Entities;
using DataLayer.Interfaces;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.WorkTemp
{
    public class WorkMessageUser : IWorkMessageUser
    {
        private ChatContext _dataContext;
        private MessageUserRepository _userRepository;
        public WorkMessageUser(DbContextOptions<ChatContext> connectionString)
        {
            _dataContext = new ChatContext(connectionString);
        }
        public IRepository<MessageUserEntity> MessageUsers { get { return _userRepository = new MessageUserRepository(_dataContext); } }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Save()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
