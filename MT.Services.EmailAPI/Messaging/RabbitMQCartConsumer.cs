using MT.Services.EmailAPI.Models;
using MT.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MT.Services.EmailAPI.Messaging;

public class RabbitMQCartConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private IConnection _connection;
    private IChannel _channel;
    private string emailCartQueue;

    public RabbitMQCartConsumer(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(emailService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;

        emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
        _channel.QueueDeclareAsync(emailCartQueue, false, false, false, null).Wait();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel: _channel);

        consumer.Received += async (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            ShoppingCartDTO shoppingCartObj = JsonConvert.DeserializeObject<ShoppingCartDTO>(content);

            await HandleMessage(shoppingCartObj);
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(emailCartQueue, false, consumer);
    }

    private async Task HandleMessage(ShoppingCartDTO shoppingCartObj)
    {
        await _emailService.EmailCartAndLogAsync(shoppingCartObj);
    }
}