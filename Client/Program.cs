using Network;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientNew
{
    public class Program
    {
        public static void Main(string[] args)
        {


            SendMessage(args[0], args[1]);
        }

        public static void SendMessage(string From, string ip)
        {

            UdpClient udpClient = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);


            while (true)
            {
                string messageText;

                do
                {
                    Console.Clear();
                    Console.WriteLine("Введите сообщение. ");
                    messageText = Console.ReadLine();

                }
                while (string.IsNullOrEmpty(messageText));
                Message message = new Message() { Text = messageText, NicNameFrom = From, NicNameTo = "Server", DateTime = DateTime.Now };
                string json = message.SerializeMessageToJson();

                byte[] date = Encoding.UTF8.GetBytes(json);
                udpClient.Send(date, date.Length, iPEndPoint);


                // Ожидание подтверждения о доставке сообщения от сервера
                byte[] confirmationBuffer = udpClient.Receive(ref iPEndPoint);
                string confirmationMessage = Encoding.UTF8.GetString(confirmationBuffer);
                Console.WriteLine("Получено подтверждение о доставке сообщения от сервера: " + confirmationMessage);

                // Отображение подтверждения о том, что сообщение было успешно обработано сервером
                byte[] responseBuffer = udpClient.Receive(ref iPEndPoint);
                string responseMessage = Encoding.UTF8.GetString(responseBuffer);
                Console.WriteLine("Ответ от сервера: " + responseMessage);

            }

        }
    }
}
