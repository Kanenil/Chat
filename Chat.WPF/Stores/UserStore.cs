using Chat.Domain.Commands;
using Chat.Domain.Models;
using Chat.Domain.Queries;
using Chat.EntityFramework.Commands;
using Chat.WPF.MVVM.ViewModels;
using Chat.WPF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WPF.Stores
{
    public class UserStore
    {
        private readonly IGetUserByLoginOrEmailQuery _getUserByLoginOrEmailQuery;
        private readonly IGetAllUsersQuery _getAllUsersQuery;
        private readonly ICreateUserCommand _createUserCommand;
        private readonly IUpdateUserCommand _updateUserCommand;
        private readonly ISendMessageCommand _sendMessageCommand;
        private readonly IGetLastMessageFromToQuery _getLastMessageFromQuery;
        private readonly IGetAllMessagesFromToQuery _getAllMessagesFromToQuery;
        private readonly ServerConnection _serverConnection;

        private User _loginedUser;
        public User LoginedUser { get { return _loginedUser; } set { _loginedUser = value; LoginedUserChanged?.Invoke(); }  }

        private readonly List<User> _users;
        public IEnumerable<User> Users => _users;
        private readonly List<string> _connectedUsers;
        public IEnumerable<string> ConnectedUsers => _connectedUsers;
        private readonly List<MessageUser> _messageUsers;
        public IEnumerable<MessageUser> MessageUsers => _messageUsers;

        public event Action UsersLoaded;
        public event Action LoginedUserChanged;
        public event Action<string> MessagesLoaded;
        public event Action ConnectedUsersChanged;

        public UserStore(IGetUserByLoginOrEmailQuery getUserByLoginOrEmailQuery, 
                         IGetAllUsersQuery getAllUsersQuery,
                         ICreateUserCommand createUserCommand,
                         IUpdateUserCommand updateUserCommand,
                         ISendMessageCommand sendMessage,
                         IGetLastMessageFromToQuery messageFromToQuery,
                         IGetAllMessagesFromToQuery getAllMessagesFromToQuery,
                         ServerConnection serverConnection)
        {
            _getUserByLoginOrEmailQuery = getUserByLoginOrEmailQuery;
            _getAllUsersQuery = getAllUsersQuery;
            _createUserCommand = createUserCommand;
            _updateUserCommand = updateUserCommand;
            _sendMessageCommand = sendMessage;
            _getLastMessageFromQuery = messageFromToQuery;
            _getAllMessagesFromToQuery = getAllMessagesFromToQuery;
            _serverConnection = serverConnection;

            _users = new List<User>();
            _messageUsers = new List<MessageUser>();
            _connectedUsers = new List<string>();
        }

        public async Task Load()
        {
            IEnumerable<User> users = await _getAllUsersQuery.Execute();

            _users.Clear();
            _users.AddRange(users);

            if (_serverConnection.IsConnected)
            {
                _serverConnection.DataReceived += ServerConnection_DataReceived;
                _serverConnection.ConnectedUsersChanged += ServerConnection_ConnectedUsersChanged;
                await _serverConnection.GetAllConnectedUsers(LoginedUser.Login);
            }
            else
                UsersLoaded?.Invoke();
        }

        public async Task LoadMessages(int userId)
        {
            IEnumerable<MessageUser> messages = await _getAllMessagesFromToQuery.Execute(LoginedUser.Id, userId);

            _messageUsers.Clear();
            _messageUsers.AddRange(messages);

            MessagesLoaded?.Invoke(userId.ToString());
        }

        private void ServerConnection_ConnectedUsersChanged()
        {
            _connectedUsers.Clear();
            _connectedUsers.AddRange(_serverConnection.ConnectedUsers);

            ConnectedUsersChanged?.Invoke();
        }

        private async void ServerConnection_DataReceived()
        {
            if (_serverConnection.LastMessage.Split(' ').Contains("connected"))
            {
                await _serverConnection.GetAllConnectedUsers(LoginedUser.Login);
            }
            else if (_serverConnection.LastMessage.Split(' ').Contains("disconnected"))
            {
                await _serverConnection.GetAllConnectedUsers(LoginedUser.Login);
            }
            else
            {
                await ConfigureUsers(_serverConnection.LastMessage);
            }
        }

        private async Task ConfigureUsers(string login)
        {
            var user = _users.FirstOrDefault(x => x.Login == login);

            if (user == null)
            {
                var users = await _getAllUsersQuery.Execute();
                _users.Clear();
                _users.AddRange(users);
                user = _users.FirstOrDefault(x => x.Login == login);
            }

            IEnumerable<MessageUser> messages = await _getAllMessagesFromToQuery.Execute(LoginedUser.Id, user.Id);

            _messageUsers.Clear();
            _messageUsers.AddRange(messages);

            MessagesLoaded?.Invoke(login);

        }

        public async Task Login(User user)
        {
            await _createUserCommand.Execute(user);
            LoginedUser = await GetUserByLoginOrEmail(user.Login);
        }

        public async Task<User> GetUserByLoginOrEmail(string searchBy)
        {
            return await _getUserByLoginOrEmailQuery.Execute(searchBy);
        }

        public async Task UpdateUser(User user)
        {
            await _updateUserCommand.Execute(user);
            LoginedUser = user;
        }

        public async Task SendMessage(MessageUser messageUser)
        {
            await _sendMessageCommand.Execute(messageUser);
        }

        public async Task<MessageUser> GetLastMessageFrom(int fromId, int toId)
        {
            return await _getLastMessageFromQuery.Execute(fromId, toId);
        }

        public async Task<IEnumerable<MessageUser>> GetAllMessagesFrom(int fromId, int toId)
        {
            return await _getAllMessagesFromToQuery.Execute(fromId, toId);
        }
    }
}
