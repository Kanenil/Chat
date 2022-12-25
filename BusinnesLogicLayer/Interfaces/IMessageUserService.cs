using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.Interfaces
{
    public interface IMessageUserService<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetCount(int count);
        Task<IEnumerable<T>> Find(int fromId, int toId);
        Task<T> FindLast(int fromId, int toId);
        Task AddItemAsync(T item);
        Task DeleteDTO(T id);
    }
}
