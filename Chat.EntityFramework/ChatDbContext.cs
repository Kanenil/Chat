using Chat.EntityFramework.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions options) : base(options) { }
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<MessageDTO> Messages { get; set; }
        public DbSet<MessageUserDTO> MessageUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MessageUserDTO>(messageUser =>
            {
                messageUser.HasKey(ur => new { ur.UserToId, ur.UserFromId, ur.MessageId });
            });

        }
    }
}
