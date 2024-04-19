using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace Network
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string NicNameFrom { get; set; }
        public string NicNameTo { get; set; }
        public bool IsDelivered { get; set; } // добавляем новое свойство для статуса доставки

        public void MarkAsDelivered()
        {
            IsDelivered = true;
        }

        public string SerializeMessageToJson() => JsonSerializer.Serialize(this);

        public static Message? DeseriaLizeFromJson(string message) => JsonSerializer.Deserialize<Message>(message);

        public void Print()
        {
            Console.WriteLine(ToString);
        }

        public override string ToString()
        {
            return ($"{this.DateTime} получено сообщение {this.Text} от {this.NicNameFrom}");
        }
    }
}
