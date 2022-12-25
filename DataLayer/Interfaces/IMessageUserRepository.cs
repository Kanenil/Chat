using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IMessageUserRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetCount(int count);
        Task<IEnumerable<T>> Find(int fromId, int toId);
        Task<T> FindLast(int fromId, int toId);
        Task Create(T item);
        void Delete(T id);
    }
}
