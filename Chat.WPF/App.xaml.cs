using Chat.Domain.Commands;
using Chat.Domain.Queries;
using Chat.EntityFramework;
using Chat.EntityFramework.Commands;
using Chat.EntityFramework.Queries;
using Chat.WPF.Commands;
using Chat.WPF.HostBuilders;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Server;
using Chat.WPF.Services;
using Chat.WPF.Services.Interface;
using Chat.WPF.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .AddDbContext()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IGetUserByLoginOrEmailQuery, GetUserByLoginOrEmailQuery>();
                    services.AddSingleton<IGetAllUsersQuery, GetAllUsersQuery>();
                    services.AddSingleton<ICreateUserCommand, CreateUserCommand>();
                    services.AddSingleton<IUpdateUserCommand, UpdateUserCommand>();
                    services.AddSingleton<ISendMessageCommand, EntityFramework.Commands.SendMessageCommand>();
                    services.AddSingleton<IGetLastMessageFromToQuery, GetLastMessageFromToQuery>();
                    services.AddSingleton<IGetAllMessagesFromToQuery, GetAllMessagesFromToQuery>();
                    services.AddSingleton<IDeleteUserCommand, DeleteUserCommand>();
                    services.AddSingleton<IDeleteAllUserMessageCommand, DeleteAllUserMessageCommand>();

                    services.AddSingleton<INavigationService>(s => CreateLoginNavigationService(s));
                    services.AddSingleton<CloseModalNavigationService>();

                    services.AddSingleton<ModalNavigationStore>();
                    services.AddSingleton<UserStore>();
                    services.AddSingleton<NavigationStore>();
                    services.AddSingleton<ServerConnection>();

                    services.AddSingleton<LoginViewModel>(CreateLoginViewModel);
                    services.AddSingleton<RegistrationViewModel>(CreateRegistrationViewModel);
                    services.AddSingleton<HomeViewModel>(CreateHomeViewModel);
                    services.AddSingleton<SettingsViewModel>(CreateSettingsViewModel);
                    services.AddSingleton<ChangeUsernameViewModel>(CreateChangeUsernameViewModel);
                    services.AddSingleton<ChangeEmailViewModel>(CreateChangeEmailViewModel);
                    services.AddSingleton<ChangePasswordViewModel>(CreateChangePasswordViewModel);
                    services.AddSingleton<DeleteUserViewModel>(CreateDeleteUserViewModel);
                    services.AddSingleton<ConnectionLostViewModel>(CreateConncetionLostViewModel);
                    services.AddSingleton<MainViewModel>();

                    services.AddSingleton<MainWindow>((services) => new MainWindow(services.GetRequiredService<ServerConnection>())
                    {
                        DataContext = services.GetRequiredService<MainViewModel>()
                    });
                })
                .Build();
        }

        private DeleteUserViewModel CreateDeleteUserViewModel(IServiceProvider s)
        {
            CompositeNavigationService navigationService = new CompositeNavigationService(
                s.GetRequiredService<CloseModalNavigationService>(),
                CreateSettingsNavigationService(s));

            return new DeleteUserViewModel(
                s.GetRequiredService<UserStore>(),
                navigationService,
                    new LogoutCommand(new CompositeNavigationService(
                        s.GetRequiredService<CloseModalNavigationService>(),
                        CreateLoginNavigationService(s))
                ,s.GetRequiredService<ServerConnection>())
                );
        }

        private ChangePasswordViewModel CreateChangePasswordViewModel(IServiceProvider s)
        {
            CompositeNavigationService navigationService = new CompositeNavigationService(
                s.GetRequiredService<CloseModalNavigationService>(),
                CreateSettingsNavigationService(s));

            return new ChangePasswordViewModel(
                s.GetRequiredService<UserStore>(),
                navigationService
                );
        }

        private ChangeEmailViewModel CreateChangeEmailViewModel(IServiceProvider s)
        {
            CompositeNavigationService navigationService = new CompositeNavigationService(
                s.GetRequiredService<CloseModalNavigationService>(),
                CreateSettingsNavigationService(s));

            return new ChangeEmailViewModel(
                s.GetRequiredService<UserStore>(),
                navigationService
                );
        }

        private ConnectionLostViewModel CreateConncetionLostViewModel(IServiceProvider s)
        {
            CompositeNavigationService navigationService = new CompositeNavigationService(
                s.GetRequiredService<CloseModalNavigationService>(),
                CreateLoginNavigationService(s));

            return new ConnectionLostViewModel(
                navigationService
                );
        }

        private HomeViewModel CreateHomeViewModel(IServiceProvider s)
        {
            return HomeViewModel.LoadViewModel(
                s.GetRequiredService<UserStore>(),
                CreateSettingsNavigationService(s),
                s.GetRequiredService<ServerConnection>(),
                new ModalNavigationService<ConnectionLostViewModel>(
                    s.GetRequiredService<ModalNavigationStore>(),
                    () => s.GetRequiredService<ConnectionLostViewModel>())
                );
        }
        private RegistrationViewModel CreateRegistrationViewModel(IServiceProvider s)
        {
            return new RegistrationViewModel(
                CreateLoginNavigationService(s),
                s.GetRequiredService<UserStore>(),
                CreateHomeNavigationService(s),
                s.GetRequiredService<ServerConnection>()
                );
        }
        private SettingsViewModel CreateSettingsViewModel(IServiceProvider s)
        {
            return new SettingsViewModel(
                s.GetRequiredService<UserStore>(),
                CreateHomeNavigationService(s),
                CreateLoginNavigationService(s),
                new ModalNavigationService<ChangeUsernameViewModel>(
                    s.GetRequiredService<ModalNavigationStore>(),
                    () => s.GetRequiredService<ChangeUsernameViewModel>()),
                new ModalNavigationService<ChangeEmailViewModel>(
                    s.GetRequiredService<ModalNavigationStore>(),
                    () => s.GetRequiredService<ChangeEmailViewModel>()),
                new ModalNavigationService<ChangePasswordViewModel>(
                    s.GetRequiredService<ModalNavigationStore>(),
                    () => s.GetRequiredService<ChangePasswordViewModel>()),
                new ModalNavigationService<DeleteUserViewModel>(
                    s.GetRequiredService<ModalNavigationStore>(),
                    () => s.GetRequiredService<DeleteUserViewModel>()),
                s.GetRequiredService<ServerConnection>()
                );
        }
        private LoginViewModel CreateLoginViewModel(IServiceProvider serviceProvider)
        {
            return new LoginViewModel(
                new NavigationService<RegistrationViewModel>(
                    serviceProvider.GetRequiredService<NavigationStore>(),
                    () => serviceProvider.GetRequiredService<RegistrationViewModel>()),
                    serviceProvider.GetRequiredService<UserStore>(),
                    CreateHomeNavigationService(serviceProvider),
                    serviceProvider.GetRequiredService<ServerConnection>()
                );
        }
        private ChangeUsernameViewModel CreateChangeUsernameViewModel(IServiceProvider serviceProvider)
        {
            CompositeNavigationService navigationService = new CompositeNavigationService(
                serviceProvider.GetRequiredService<CloseModalNavigationService>(),
                CreateSettingsNavigationService(serviceProvider));

            return new ChangeUsernameViewModel(
                serviceProvider.GetRequiredService<UserStore>(),
                navigationService
                );
        }

        private INavigationService CreateSettingsNavigationService(IServiceProvider s)
        {
            return new NavigationService<SettingsViewModel>(
                    s.GetRequiredService<NavigationStore>(),
                    () => s.GetRequiredService<SettingsViewModel>());
        }
        private INavigationService CreateLoginNavigationService(IServiceProvider s)
        {
            return new NavigationService<LoginViewModel>(
                s.GetRequiredService<NavigationStore>(),
                () => s.GetRequiredService<LoginViewModel>());
        }
        private INavigationService CreateHomeNavigationService(IServiceProvider s)
        {
            return new NavigationService<HomeViewModel>(
                s.GetRequiredService<NavigationStore>(),
                () => s.GetRequiredService<HomeViewModel>());
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            ChatDbContextFactory chatDbContextFactory =
                _host.Services.GetRequiredService<ChatDbContextFactory>();
            using (ChatDbContext context = chatDbContextFactory.Create())
            {
                context.Database.Migrate();
            }

            INavigationService initialNavigationService = _host.Services.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }

    }
}
