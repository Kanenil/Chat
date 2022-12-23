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
    public class UserRepository : IRepository<UserEntity>
    {
        private ChatContext _dataContext;
        public UserRepository(ChatContext context)
        {
            _dataContext = context;
        }
        public async Task Create(UserEntity item)
        {
            if (item != null)
                await _dataContext.Users.AddAsync(item);
        }

        public void Delete(UserEntity id)
        {
            _dataContext.Users.Remove(id);
        }

        public async Task<UserEntity> Find(UserEntity id)
        {
            var product = await _dataContext.Users.FindAsync(id);
            if (product != null)
                return product;
            throw new InvalidOperationException();
        }

        public IEnumerable<UserEntity> GetAll()
        {
            var list = _dataContext.Users;
            foreach (var entity in list)
                _dataContext.Entry(entity).State = EntityState.Detached;
            return list;
        }

        public int GetId(UserEntity item)
        {
            var tempItem = _dataContext.Users.Where(e => e.Login == item.Login || e.Email == item.Email).First();
            return tempItem.Id;
        }

        public void Update(UserEntity item)
        {
            if (item != null)
            {
                var newItem = _dataContext.Users.Where(x => x.Id == item.Id).First();
                newItem.Login = item.Login;
                newItem.EmailConfirmed = item.EmailConfirmed;
                newItem.Email = item.Email;
                newItem.Password = item.Password;
            }
        }
    }
}
