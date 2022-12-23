using DataLayer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Data
{
    public class ChatContext : DbContext
    {
        private ChatContext _dataContext;
        public DbSet<UserEntity> Users { get; set; }
        public ChatContext(DbContextOptions<ChatContext> connectionString) : base(connectionString)
        {
            Database.Migrate();
        }
        public ChatContext(ChatContext context)
        {
            this._dataContext = context;
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ChatDB;Integrated Security=True;");
        //}
    }
}
