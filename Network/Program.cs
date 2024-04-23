using Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Network_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine("Для остановки сервера нажмите клавишу 'q'.");
            // Запуск сервера в отдельном потоке
            ThreadPool.QueueUserWorkItem(obj =>
            {
                Server("Dima", cancellationTokenSource.Token);
            });

            // Ожидание нажатия клавиши 'q' для остановки сервера
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
            cancellationTokenSource.Cancel(); // Отмена работы сервера
            Console.WriteLine("Сервер остановлен.");
        }

        public void task1()
        {
            Message msg = new Message() { Text = "Hello", DateTime = DateTime.Now, NicNameFrom = "Dima", NicNameTo = "ALL" };
            string json = msg.SerializeMessageToJson();
            Console.WriteLine(json);
            Message? msgDeserialized = Message.DeseriaLizeFromJson(json);
        }

        public static void Server(string name, CancellationToken cancellationToken)
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Сервер ждет сообщение от клиента");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    byte[] buffer = udpClient.Receive(ref iPEndPoint);
                    var messageText = Encoding.UTF8.GetString(buffer);

                    ThreadPool.QueueUserWorkItem(obj =>
                    {
                        Message message = Message.DeseriaLizeFromJson(messageText);
                        Console.WriteLine($"{message.DateTime} получено сообщение {message.Text} от {message.NicNameFrom}");
                        message.Print();

                        // Передача сообщения клиенту
                        byte[] confirmation = Encoding.UTF8.GetBytes("Получено сообщение");
                        udpClient.Send(confirmation, confirmation.Length, iPEndPoint);
                    });
                }
                catch (SocketException)
                {
                    // Обработка исключения SocketException при закрытии сервера
                    break;
                }
            }
            // Закрытие UdpClient при завершении работы сервера
            udpClient.Close();
        }
    }
}
