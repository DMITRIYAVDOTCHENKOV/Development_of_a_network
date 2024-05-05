using ClientNew;
using Moq;
using NUnit.Framework;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client.Tests
{
    [TestFixture]
    public class MessageHandlerTests
    {
        [Test]
        public async Task SendMessage_ValidInput_ReturnsResponse()
        {
            // Arrange
            var messageHandler = new MessageHandler();
            string from = "Dima";
            string message = "Hello, server!";
            string ip = "127.0.0.1";
            int port = 12345;

            // Act
            string response = await messageHandler.SendMessage(from, message, ip, port);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo($"Received message: {message}"));
        }

        [Test]
        public async Task SendMessage_ServerUnavailable_ReturnsNull()
        {
            // Arrange
            var messageHandler = new MessageHandler();
            string from = "Dima";
            string message = "Hello, server!";
            string ip = "192.168.1.1"; // несуществующий IP-адрес
            int port = 12345;

            // Act
            string response = await messageHandler.SendMessage(from, message, ip, port);

            // Assert
            Assert.That(response, Is.Null);
        }
    }
}
