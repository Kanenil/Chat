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
    public class GetAllUsersQuery : IGetAllUsersQuery
    {
        private readonly ChatDbContextFactory _contextFactory;

        public GetAllUsersQuery(ChatDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<User>> Execute()
        {
            using (ChatDbContext context = _contextFactory.Create())
            {
                IEnumerable<UserDTO> userDtos = await context.Users.ToListAsync();

                return userDtos.Select(u => new User(u.Id, u.Login, u.Email, u.Password, u.Photo, u.EmailConfirmed));
            }
        }
    }
}
