using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServerConcole
{
    internal class Program
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        static readonly Dictionary<string, int> clients_login = new Dictionary<string, int>(); 
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            int count = 1;

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
            TcpListener serverSocket = new TcpListener(ip, port);  //запускаємо сервер
            serverSocket.Start();
            Console.WriteLine("Запуск сервака {0}:{1}", ip, port);

            while (true)
            {
                TcpClient client = serverSocket.AcceptTcpClient();
                lock (_lock)
                {
                    list_clients.Add(count, client); 
                }
                Console.WriteLine("Появився на сервері новий клієнт {0}", client.Client.RemoteEndPoint); 

                Thread t = new Thread(handle_clients);  
                t.Start(count);

                count++;
            }
        }
        public static void handle_clients(object c)
        {
            int id = (int)c;
            TcpClient client;
            lock (_lock)
            {
                client = list_clients[id];
            }
            while (true) 
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];

                int byte_count = stream.Read(buffer); 
                if (byte_count == 0)
                    break; 

                string data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                var split = data.Split(' ');

                //Commands: rename to "name"
                //          send to "name" message from "name"

                if (split[0] == "rename")
                {
                    clients_login.Add(split[2], id);
                    Console.WriteLine("Клієнт {0} викликав команду rename", client.Client.RemoteEndPoint);
                }
                else if (split[0] == "send")
                {
                    try
                    {
                        broadcast(clients_login[split[2]], split[5]);
                        Console.WriteLine("Клієнт {0} викликав команду send", client.Client.RemoteEndPoint);
                    }
                    catch 
                    {
                        Console.WriteLine("Клієнт {0} викликав команду send, приймач оффлайн", client.Client.RemoteEndPoint);
                    }
                }
                
            }
            lock (_lock)
            {
                Console.WriteLine("Клієнт {0} відключився", client.Client.RemoteEndPoint);
                foreach (var item in clients_login)
                {
                    if (item.Value == id)
                    {
                        clients_login.Remove(item.Key);
                        break;
                    }
                }
                list_clients.Remove(id);
            }
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public static void broadcast(int data,string message) 
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            lock (_lock) 
            {
                var client = list_clients[data];
                NetworkStream stream = client.GetStream();
                stream.Write(buffer);
            }
        }
    }
}