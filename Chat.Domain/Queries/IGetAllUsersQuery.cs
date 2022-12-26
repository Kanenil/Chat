using Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Queries
{
    public interface IGetAllUsersQuery
    {
        Task<IEnumerable<User>> Execute();
    }
}
