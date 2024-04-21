using Network;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Network_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server("Dima");
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey();
        }
        public void task1()
        {
            Message msg = new Message() { Text = "Hello", DateTime = DateTime.Now, NicNameFrom = "Dima", NicNameTo = "ALL" };
            string json = msg.SerializeMessageToJson();
            Console.WriteLine(json);
            Message? msgDeserialized = Message.DeseriaLizeFromJson(json);
        }


        public static void Server(string name)
        {

            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
          

            Console.WriteLine("Сервер ждет сообщение от клиента");

            while (true)
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
        }
    }
}