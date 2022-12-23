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
    public class WorkUser : IWorkUser
    {
        private ChatContext _dataContext;
        private UserRepository _userRepository;
        public WorkUser(DbContextOptions<ChatContext> connectionString)
        {
            _dataContext = new ChatContext(connectionString);
        }
        public IRepository<UserEntity> Users { get { return _userRepository = new UserRepository(_dataContext); } }

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
