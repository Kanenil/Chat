using Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Queries
{
    public interface IGetAllMessagesFromToQuery
    {
        Task<IEnumerable<MessageUser>> Execute(int fromId, int toId);
    }
}
