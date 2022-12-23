using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Infrastructure;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
using ClientWPF.ViewModels;
using ClientWPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private readonly string connectionString;
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            connectionString = ConfigurationManager.ConnectionStrings["ChatDB"].ConnectionString;
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(ServiceCollection collection)
        {
            collection.AddSingleton(typeof(IService<UserDTO>), typeof(UserService));
            collection.AddSingleton(typeof(LoginViewModel));
            collection.AddSingleton<MainWindow>();
            ConfigurationBLL.ConfigureServices(collection, connectionString);
            ConfigurationBLL.AddDependecy(collection);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = (Window)_serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
