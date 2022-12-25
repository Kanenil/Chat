using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientWPF.Server
{
    public class ServerConnection
    {
        TcpClient client = new TcpClient();
        NetworkStream ns;
        Thread thread;
        public string LastMessage { get; private set; }
        public List<string> ConnectedUsers { get; private set; }
        public bool IsConnected => client.Connected;
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
        }
        public void CloseConnection()
        {
            if (client.Connected)
            {
                client.Client.Shutdown(SocketShutdown.Send);
                ConnectedUsers.Clear();
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
                        OnConnectedUsersChanged();
                    }
                    else
                    {
                        LastMessage = data;
                        OnDataReceived();
                    }
                }
            }
            catch
            {

            }
        }
        private void OnDataReceived()
        {
            DataReceived?.Invoke();
        }
        private void OnConnectedUsersChanged()
        {
            ConnectedUsersChanged?.Invoke();
        }
    }
}
