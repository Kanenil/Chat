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
    public class GetUserByLoginOrEmailQuery : IGetUserByLoginOrEmailQuery
    {
        private readonly ChatDbContextFactory _contextFactory;

        public GetUserByLoginOrEmailQuery(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<User> Execute(string searchBy)
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                UserDTO userDtos = (await context.Users.ToListAsync()).FirstOrDefault(u=>u.Login.ToLower() == searchBy.ToLower() || u.Email.ToLower() == searchBy.ToLower());
                if (userDtos != null)
                    return new User(userDtos.Id, userDtos.Login, userDtos.Email, userDtos.Password, userDtos.Photo, userDtos.EmailConfirmed);
                return null;
            }
        }
    }
}
