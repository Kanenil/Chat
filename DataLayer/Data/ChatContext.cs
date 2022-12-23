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
            Database.EnsureCreated();
            //Database.Migrate();
        }
        public ChatContext(ChatContext context)
        {
            this._dataContext = context;
        }
        public ChatContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=chatDatabase.mssql.somee.com;Database=chatDatabase;User Id=gonel_SQLLogin_1;Password=ss8zjgf2bq;TrustServerCertificate=True;");
        }
    }
}
