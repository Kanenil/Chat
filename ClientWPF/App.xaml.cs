using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Infrastructure;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.Services;
using ClientWPF.Services;
using ClientWPF.Stores;
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

            services.AddSingleton<AccountStore>();
            services.AddSingleton<PeopleStore>();
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<ModalNavigationStore>();

            services.AddSingleton(typeof(IService<UserDTO>), typeof(UserService));
            services.AddSingleton(typeof(IService<MessageUserDTO>), typeof(MessageUserService));
            ConfigurationBLL.ConfigureServices(services, connectionString);
            ConfigurationBLL.AddDependecy(services);

            services.AddSingleton<INavigationService>(s => CreateHomeNavigationService(s));
            services.AddSingleton<CloseModalNavigationService>();

            services.AddTransient<HomeViewModel>(CreateHomeViewModel);
            services.AddTransient<RegistrationViewModel>(CreateRegistartionViewModel);
            services.AddTransient<SettingsViewModel>(CreateSettingsViewModel);
            services.AddTransient<AccountViewModel>(s => new AccountViewModel(
                s.GetRequiredService<AccountStore>(),
                CreateHomeNavigationService(s),
                CreateSettingsNavigationService(s),
                s.GetRequiredService<IService<UserDTO>>(),
                s.GetRequiredService<IService<MessageUserDTO>>())
                );
            services.AddTransient<PeopleListingViewModel>(s => new PeopleListingViewModel(
                s.GetRequiredService<PeopleStore>(),
                CreateAddPersonNavigationService(s)));
            services.AddTransient<AddPersonViewModel>(s => new AddPersonViewModel(
                s.GetRequiredService<PeopleStore>(),
                s.GetRequiredService<CloseModalNavigationService>()
                ));

            services.AddTransient<ChangeUsernameViewModel>(CreateChangeUsernameViewModel);

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            INavigationService initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        private INavigationService CreateHomeNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<HomeViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<HomeViewModel>());
        }

        private INavigationService CreateSettingsNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<SettingsViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<SettingsViewModel>());
        }

        private INavigationService CreateLoginNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigationService<LoginViewModel>(
                serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<LoginViewModel>());
        }

        private INavigationService CreateAddPersonNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigationService<AddPersonViewModel>(
                serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<AddPersonViewModel>());
        }

        private INavigationService CreateAccountNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<AccountViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<AccountViewModel>());
        }

        private INavigationService CreatePeopleListingNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<PeopleListingViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<PeopleListingViewModel>());
        }
        private INavigationService CreateRegistartionNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<RegistrationViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<RegistrationViewModel>());
        }

        private INavigationService CreateChangeNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigationService<ChangeUsernameViewModel>(
                serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<ChangeUsernameViewModel>());
        }

        private HomeViewModel CreateHomeViewModel(IServiceProvider serviceProvider)
        {
            return new HomeViewModel(
                serviceProvider.GetRequiredService<AccountStore>(),
                serviceProvider.GetRequiredService<IService<UserDTO>>(),
                CreateAccountNavigationService(serviceProvider),
                CreateRegistartionNavigationService(serviceProvider));
        }

        private RegistrationViewModel CreateRegistartionViewModel(IServiceProvider serviceProvider)
        {
            return new RegistrationViewModel(
                serviceProvider.GetRequiredService<AccountStore>(),
                CreateAccountNavigationService(serviceProvider),
                CreateHomeNavigationService(serviceProvider),
                serviceProvider.GetRequiredService<IService<UserDTO>>());
        }

        private SettingsViewModel CreateSettingsViewModel(IServiceProvider serviceProvider)
        {
            return new SettingsViewModel(
                CreateAccountNavigationService(serviceProvider),
                serviceProvider.GetRequiredService<AccountStore>(),
                serviceProvider.GetRequiredService<IService<UserDTO>>(),
                CreateHomeNavigationService(serviceProvider),
                CreateChangeNavigationService(serviceProvider));
        }

        private ChangeUsernameViewModel CreateChangeUsernameViewModel(IServiceProvider serviceProvider)
        {
            CompositeNavigationService navigationService = new CompositeNavigationService(
                serviceProvider.GetRequiredService<CloseModalNavigationService>(),
                CreateSettingsNavigationService(serviceProvider));

            return new ChangeUsernameViewModel(
                serviceProvider.GetRequiredService<IService<UserDTO>>(),
                serviceProvider.GetRequiredService<AccountStore>(),
                navigationService);
        }
    }
}
