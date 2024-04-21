using Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите сообщение (для выхода введите 'Exit'):");
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }
                SendMessage("Dima", input);
            }
        }

        public static void SendMessage(string From, string messageText, string ip = "127.0.0.1", int port = 12345)
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Message message = new Message() { Text = messageText, NicNameFrom = From, NicNameTo = "Server", DateTime = DateTime.Now };
            string json = message.SerializeMessageToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);

            udpClient.Send(data, data.Length, iPEndPoint);

            byte[] buffer = udpClient.Receive(ref iPEndPoint);
            var answer = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(answer);

            udpClient.Close();
        }
    }
}
