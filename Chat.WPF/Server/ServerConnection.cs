using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Chat.WPF.Server
{
    public class ServerConnection
    {
        TcpClient client = new TcpClient();
        NetworkStream ns;
        Thread thread;
        public string LastMessage { get; private set; }
        public List<string> ConnectedUsers { get; private set; }

        public async Task<bool> ConnectAndRename(string login)
        {
            try
            {
                await Connect();

                try
                {
                    await Rename(login);

                    if (IsConnected)
                    {
                        _checkConnectTimer.Elapsed += OnCheckConnectionTimer;
                        _checkConnectTimer.AutoReset = true;
                        _checkConnectTimer.Enabled = true;
                        return true;
                    }
                    else
                        throw new Exception("Already logined");

                }
                catch 
                {
                    return false;
                }

            }
            catch 
            {
                throw new Exception("Connection error");
            }
        }

        public async Task Connect()
        {
            client = new TcpClient();
            string fileName = "config.txt";
            IPAddress ip;
            int port;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    ip = IPAddress.Parse(await sr.ReadLineAsync());
                    port = int.Parse(await sr.ReadLineAsync());
                }
            }
            await client.ConnectAsync(ip, port);
            ns = client.GetStream();
            thread = new Thread(o => DataResive());
            thread.Start();

            _checkConnectTimer.Elapsed += OnCheckConnectionTimer;
            _checkConnectTimer.AutoReset = true;
            _checkConnectTimer.Start();
        }
        public void CloseConnection(bool notevicate = true)
        {
            if (!notevicate)
            {
                _checkConnectTimer.Elapsed -= OnCheckConnectionTimer;
                _checkConnectTimer.Stop();
            }

            if (client.Connected)
                client.Client.Shutdown(SocketShutdown.Send);
            ConnectedUsers.Clear();
        }

        private System.Timers.Timer _checkConnectTimer = new System.Timers.Timer(5000);
        private void OnCheckConnectionTimer(object? sender, ElapsedEventArgs e)
        {
            if (IsConnected == false)
            {
                _checkConnectTimer.Elapsed -= OnCheckConnectionTimer;
                _checkConnectTimer.Stop();
                OnConnectionLost?.Invoke();
            }
        }

        public async Task Rename(string login)
        {
            var buffer = Encoding.ASCII.GetBytes($"rename to {login} ");
            await ns.WriteAsync(buffer);
        }
        public async Task GetAllConnectedUsers(string login)
        {
            var buffer = Encoding.ASCII.GetBytes($"get all conected users to {login} ");
            await ns.WriteAsync(buffer);
        }
        public async Task SendMessage(string login, string from)
        {
            while (!client.Connected) { }

            var buffer = Encoding.ASCII.GetBytes($"send to {login} message from {from} ");
            await ns.WriteAsync(buffer);
        }
        public event Action DataReceived;
        public event Action ConnectedUsersChanged;
        public event Action OnConnectionLost;
        private async void DataResive()
        {
            NetworkStream ns = client.GetStream();
            var receiveBytes = new byte[4096];
            int byte_count;
            try
            {
                while ((byte_count = await ns.ReadAsync(receiveBytes)) > 0)
                {
                    var data = Encoding.ASCII.GetString(receiveBytes, 0, byte_count);
                    var connected = data.Split('[', ',', ']').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    if (data[0] == '[')
                    {
                        ConnectedUsers = new List<string>();
                        foreach (var item in connected)
                            ConnectedUsers.Add(item);
                        ConnectedUsersChanged?.Invoke();
                    }
                    else
                    {
                        LastMessage = data;
                        DataReceived?.Invoke();
                    }
                }
            }
            catch
            {

            }
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (client != null && client.Client != null && client.Client.Connected)
                    {
                        /* pear to the documentation on Poll:
                         * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                         * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                         * -or- true if data is available for reading; 
                         * -or- true if the connection has been closed, reset, or terminated; 
                         * otherwise, returns false
                         */

                        // Detect if client disconnected
                        if (client.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
