using Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Queries
{
    public interface IGetLastMessageFromToQuery
    {
        Task<MessageUser> Execute(int fromId, int toId);
    }
}
