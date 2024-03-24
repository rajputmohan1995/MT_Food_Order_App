using MT.Services.AuthAPI.RabbmitMQSender;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MT.Services.AuthAPI.RabbitMQSender;

public class RabbitMQAuthMessageSender : IRabbitMQAuthMessageSender
{
    private readonly string _hostName;
    private readonly string _username;
    private readonly string _password;
    private IConnection _connection;

    public RabbitMQAuthMessageSender()
    {
        _hostName = "localhost";
        _username = "guest";
        _password = "guest";
    }

    public async Task SendMessage(object message, string queueName)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            UserName = _username,
            Password = _password
        };

        _connection = await factory.CreateConnectionAsync();

        using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queueName, false, false, false, null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body, false);
    }
}