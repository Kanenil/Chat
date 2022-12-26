using Chat.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.HostBuilders
{
    public static class AddDbContextHostBuilderExtensions
    {
        public static IHostBuilder AddDbContext(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                string connectionString = context.Configuration.GetConnectionString("mssql");

                services.AddSingleton<DbContextOptions>(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
                services.AddSingleton<ChatDbContextFactory>();
            });

            return hostBuilder;
        }
    }
}
