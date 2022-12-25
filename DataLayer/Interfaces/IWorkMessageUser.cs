using DataLayer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IWorkMessageUser : IWork
    {
        public IMessageUserRepository<MessageUserEntity> MessageUsers { get; }
    }
}
