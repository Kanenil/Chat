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
        public bool IsConnected => client.Connected;
        public void Connect()
        {
            string fileName = "config.txt";
            IPAddress ip;
            int port;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    ip = IPAddress.Parse(sr.ReadLine());
                    port = int.Parse(sr.ReadLine());
                }
            }
            client.Connect(ip, port);
            ns = client.GetStream();
            thread = new Thread(o => DataResive());
            thread.Start(client);
        }
        public void CloseConnection()
        {
            if (client.Connected)
            {
                client.Client.Shutdown(SocketShutdown.Send);
                client.Close();
            }
        }
        public void Rename(string login)
        {
            var buffer = Encoding.ASCII.GetBytes($"rename to {login}");
            ns.Write(buffer);
        }
        public void SendMessage(string login, string from)
        {
            var buffer = Encoding.ASCII.GetBytes($"send to {login} message from {from}");
            ns.Write(buffer);
        }
        public event Action DataReceived;
        private void DataResive()
        {
            NetworkStream ns = client.GetStream();
            var receiveBytes = new byte[4096];
            int byte_count;
            string data = "";
            try
            {
                while ((byte_count = ns.Read(receiveBytes)) > 0)
                {
                    LastMessage = Encoding.ASCII.GetString(receiveBytes, 0, byte_count);
                    OnDataReceived();
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
    }
}
