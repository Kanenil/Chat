using DataLayer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Data
{
    public class ChatContext : DbContext
    {
        private ChatContext _dataContext;
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<MessageUserEntity> MessageUsers { get; set; }
        public ChatContext(DbContextOptions<ChatContext> connectionString) : base(connectionString)
        {
            Database.Migrate();
        }
        public ChatContext(ChatContext context)
        {
            this._dataContext = context;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MessageUserEntity>(messageUser =>
            {
                messageUser.HasKey(ur => new { ur.UserToId, ur.UserFromId, ur.MessageId });
            });

        }
    }
}
