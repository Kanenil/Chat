using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.EntityFramework
{
    public class ChatDbContextFactory
    {
        private readonly DbContextOptions _options;

        public ChatDbContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public ChatDbContext Create()
        {
            return new ChatDbContext(_options);
        }
    }
}
