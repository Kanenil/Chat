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
            Console.WriteLine("[{0}] Запуск сервера {1}:{2}",DateTime.Now, ip, port);

            while (true)
            {
                TcpClient client = serverSocket.AcceptTcpClient();
                lock (_lock)
                {
                    list_clients.Add(count, client); 
                }
                Console.WriteLine("[{0}] Появився на сервері новий клієнт {1}", DateTime.Now, client.Client.RemoteEndPoint); 

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
                //          get all conected users to "name"

                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i] == "rename")
                    {
                        foreach (var item in clients_login)
                            broadcast(item.Value, $"connected {split[2]}");
                        clients_login.Add(split[i + 2], id);
                        Console.WriteLine("[{0}] Клієнт {1}  викликав команду rename to {2}", DateTime.Now, client.Client.RemoteEndPoint, split[2]);
                        i = i + 2;
                    }
                    else if(split[i] == "send")
                    {
                        try
                        {
                            broadcast(clients_login[split[i+2]], split[i+5]);
                            Console.WriteLine("[{0}] Клієнт {1} викликав команду send", DateTime.Now, client.Client.RemoteEndPoint);
                        }
                        catch
                        {
                            Console.WriteLine("[{0}] Клієнт {1} викликав команду send, приймач оффлайн", DateTime.Now, client.Client.RemoteEndPoint);
                        }
                        i = i + 5;
                    }
                    else if (split[i] == "get")
                    {
                        string message = "[";
                        foreach (var item in clients_login)
                            message += $"{item.Key},";
                        message.Remove(message.Length - 1);
                        message += "]";
                        broadcast(clients_login[split[i+5]], message);
                        Console.WriteLine("[{0}] Клієнт {1} викликав команду get all", DateTime.Now, client.Client.RemoteEndPoint);
                        i = i + 5;
                    }
                }


            }
            lock (_lock)
            {
                int value = 0;
                string key = "";
                foreach (var item in clients_login)
                {
                    if (item.Value == id)
                    {
                        key = item.Key;
                        value = item.Value;
                        clients_login.Remove(item.Key);
                        break;
                    }
                }
                list_clients.Remove(value);
                Console.WriteLine("[{0}] Клієнт {1} -> {2} відключився", DateTime.Now, client.Client.RemoteEndPoint, key);
                foreach (var item in clients_login)
                    broadcast(item.Value, $"disconnected {key}");
                Console.WriteLine("[{0}] Залишилось {1} Клієнт(-ів)", DateTime.Now, list_clients.Count);
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