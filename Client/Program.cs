using Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientNew
{
    // Класс для обработки сообщений
    public class MessageHandler
    {
        private UdpClientSingleton udpClientSingleton;

        public MessageHandler()
        {
            udpClientSingleton = UdpClientSingleton.GetInstance();
        }

        public async Task<string> SendMessage(string from, string messageText, string ip = "127.0.0.1", int port = 12345)
        {
            try
            {
                UdpClient udpClient = udpClientSingleton.GetUdpClient();
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                Message message = new Message() { Text = messageText, NicNameFrom = from, NicNameTo = "Server", DateTime = DateTime.Now };
                string json = message.SerializeMessageToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);

                await udpClient.SendAsync(data, data.Length, iPEndPoint);

                UdpReceiveResult result;
                try
                {
                    result = await udpClient.ReceiveAsync();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Ошибка при приеме ответа от сервера: {ex.Message}");
                    return null;
                }

                return Encoding.UTF8.GetString(result.Buffer);
            }
            catch (SocketException)
            {
                Console.WriteLine("Ошибка: сервер недоступен.");
                return null;
            }
        }
    }

    // Класс-одиночка для UdpClient
    public class UdpClientSingleton
    {
        private static UdpClientSingleton instance;
        private UdpClient udpClient;

        private UdpClientSingleton()
        {
            udpClient = new UdpClient();
        }

        public static UdpClientSingleton GetInstance()
        {
            if (instance == null)
            {
                instance = new UdpClientSingleton();
            }
            return instance;
        }

        public UdpClient GetUdpClient()
        {
            return udpClient;
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            MessageHandler messageHandler = new MessageHandler();

            while (true)
            {
                Console.WriteLine("Введите сообщение (для выхода введите 'Exit'):");
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }
                string response = await messageHandler.SendMessage("Dima", input);
                if (response != null)
                {
                    Console.WriteLine(response);
                }
            }
        }
    }
}
