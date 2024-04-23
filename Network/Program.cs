using Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Network_
{
    // Определяем интерфейс для наблюдателя
    public interface IMessageObserver
    {
        void OnMessageReceived(Message message);
    }

    // Реализуем класс для наблюдателя (в данном случае, для вывода сообщений в консоль)
    public class ConsoleMessageObserver : IMessageObserver
    {
        public void OnMessageReceived(Message message)
        {
            Console.WriteLine($"{message.DateTime} получено сообщение {message.Text} от {message.NicNameFrom}");
            message.Print();
        }
    }

    // Реализуем класс для шаблона Singleton
    public class UdpClientSingleton
    {
        private static UdpClientSingleton instance;
        private UdpClient udpClient;

        // Приватный конструктор, чтобы предотвратить создание объекта извне
        private UdpClientSingleton()
        {
            udpClient = new UdpClient(12345);
        }

        // Метод для получения экземпляра класса
        public static UdpClientSingleton GetInstance()
        {
            if (instance == null)
            {
                instance = new UdpClientSingleton();
            }
            return instance;
        }

        // Метод для получения UdpClient
        public UdpClient GetUdpClient()
        {
            return udpClient;
        }
    }

    public class ChatServer
    {
        private UdpClientSingleton udpClientSingleton;
        private IMessageObserver messageObserver;
        private CancellationToken cancellationToken;

        public ChatServer(IMessageObserver observer, CancellationToken token)
        {
            messageObserver = observer;
            cancellationToken = token;
            udpClientSingleton = UdpClientSingleton.GetInstance();
        }

        public void Start()
        {
            Console.WriteLine("Сервер ждет сообщение от клиента");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0); // Объявляем iPEndPoint здесь
                    byte[] buffer = udpClientSingleton.GetUdpClient().Receive(ref iPEndPoint);
                    var messageText = Encoding.UTF8.GetString(buffer);

                    ThreadPool.QueueUserWorkItem(obj =>
                    {
                        Message message = Message.DeseriaLizeFromJson(messageText);
                        messageObserver.OnMessageReceived(message);

                        // Передача подтверждения клиенту
                        byte[] confirmation = Encoding.UTF8.GetBytes("Получено сообщение");
                        udpClientSingleton.GetUdpClient().Send(confirmation, confirmation.Length, iPEndPoint);
                    });
                }
                catch (SocketException)
                {
                    // Обработка исключения SocketException при закрытии сервера
                    break;
                }
            }
            // Закрытие UdpClient при завершении работы сервера
            udpClientSingleton.GetUdpClient().Close();
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine("Для остановки сервера нажмите клавишу 'q'.");

            // Создаем экземпляр наблюдателя для вывода сообщений в консоль
            IMessageObserver consoleObserver = new ConsoleMessageObserver();

            // Создаем экземпляр сервера чата и запускаем его в отдельном потоке
            ChatServer chatServer = new ChatServer(consoleObserver, cancellationTokenSource.Token);
            ThreadPool.QueueUserWorkItem(obj =>
            {
                chatServer.Start();
            });

            // Ожидание нажатия клавиши 'q' для остановки сервера
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
            cancellationTokenSource.Cancel(); // Отмена работы сервера
            Console.WriteLine("Сервер остановлен.");
        }
    }
}
