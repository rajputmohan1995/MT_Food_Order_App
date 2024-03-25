
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace MT.Services.ShoppingCartAPI.RabbitMQSender;

public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
{
    private string _hostName;
    private string _username;
    private string _password;
    private IConnection _connection;

    public RabbitMQCartMessageSender()
    {
        _hostName = "localhost";
        _username = "guest";
        _password = "guest";
    }

    public async Task SendMessage(object message, string queueName)
    {
        var isConnectionExists = await ConnectionExists();

        if (isConnectionExists)
        {
            using var channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queueName, false, false, false, null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body, false);
        }
    }

    private async Task CreateConnectionAsync()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _username,
                Password = _password
            };

            _connection = await factory.CreateConnectionAsync();
        }
        catch
        { }
    }

    private async Task<bool> ConnectionExists()
    {
        if (_connection != null)
            return true;
        await CreateConnectionAsync();
        return true;
    }
}