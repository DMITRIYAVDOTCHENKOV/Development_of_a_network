using Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientNew
{
    public class Program
    {
        private static bool serverRunning = true;

        public static async Task Main(string[] args)
        {
            while (true)
            {
                if (serverRunning)
                {
                    Console.WriteLine("Введите сообщение (для выхода введите 'Exit'):");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "exit")
                    {
                        break;
                    }
                    await SendMessage("Dima", input);
                }
                else
                {
                    Console.WriteLine("Сервер не доступен. Повторная попытка подключения через 5 секунд...");
                    await Task.Delay(5000); // Подождать 5 секунд перед повторной попыткой подключения
                    serverRunning = true;
                }
            }
        }

        public static async Task SendMessage(string From, string messageText, string ip = "127.0.0.1", int port = 12345)
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Message message = new Message() { Text = messageText, NicNameFrom = From, NicNameTo = "Server", DateTime = DateTime.Now };
            string json = message.SerializeMessageToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);

            try
            {
                await udpClient.SendAsync(data, data.Length, iPEndPoint);

                UdpReceiveResult result;
                try
                {
                    result = await udpClient.ReceiveAsync();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Ошибка при приеме ответа от сервера: {ex.Message}");
                    serverRunning = false; // Пометить сервер как недоступный
                    return;
                }

                var answer = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine(answer);
            }
            catch (SocketException)
            {
                // Обработка ситуации, когда сервер остановлен
                Console.WriteLine("Ошибка: сервер недоступен.");
                serverRunning = false;
            }
            finally
            {
                udpClient.Close();
            }
        }
    }
}
