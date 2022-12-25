using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.Interfaces
{
    public interface IService<T>
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetCount(int count);
        Task AddItemAsync(object item);
        Task<T> FindDTO(T id);
        Task UpdateDTO(T item);
        Task DeleteDTO(T id);
        Task AddList(IEnumerable<T> templist);
        int GetIdDTO(T item);
    }
}
