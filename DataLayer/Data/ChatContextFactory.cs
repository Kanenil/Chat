using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Data
{
    public class ChatContextFactory : IDesignTimeDbContextFactory<ChatContext>
    {
        public ChatContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ChatContext>().UseSqlServer("Server=chatDatabase.mssql.somee.com;Database=chatDatabase;User Id=gonel_SQLLogin_1;Password=ss8zjgf2bq;TrustServerCertificate=True;");

            return new ChatContext(options.Options);
        }
    }
}
