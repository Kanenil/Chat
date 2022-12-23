using DataLayer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IWorkUser : IWork
    {
        public IRepository<UserEntity> Users { get; }
    }
}
