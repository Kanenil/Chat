using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.Infrastructure
{
    public class ConfigurationBLL
    {
        public static void ConfigureServices(ServiceCollection services, string connection)
        {
            services.AddDbContext<ChatContext>(opt => opt.UseSqlServer(connection), ServiceLifetime.Transient);
        }
        public static void AddDependecy(ServiceCollection services)
        {
            services.AddSingleton(typeof(DataLayer.Interfaces.IWorkUser), typeof(DataLayer.WorkTemp.WorkUser));
            services.AddSingleton(typeof(DataLayer.Interfaces.IWorkMessageUser), typeof(DataLayer.WorkTemp.WorkMessageUser));
        }
    }
}
