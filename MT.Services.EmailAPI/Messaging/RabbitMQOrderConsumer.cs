using MT.Services.EmailAPI.Message;
using MT.Services.EmailAPI.Models;
using MT.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MT.Services.EmailAPI.Messaging;

public class RabbitMQOrderConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private IConnection _connection;
    private IChannel _channel;
    private string newOrderTopicName;
    string queueName = "";

    public RabbitMQOrderConsumer(IConfiguration configuration, EmailService emailService)
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

        newOrderTopicName = _configuration.GetValue<string>("TopicAndQueueNames:NewOrderGeneratedTopic");
        _channel.ExchangeDeclareAsync(newOrderTopicName, ExchangeType.Fanout).Wait();
        queueName = _channel.QueueDeclareAsync().Result.QueueName;
        _channel.QueueBindAsync(queueName, newOrderTopicName, "").Wait();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel: _channel);

        consumer.Received += async (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var rewardObj = JsonConvert.DeserializeObject<RewardMessage>(content);

            await HandleMessage(rewardObj);
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queueName, false, consumer);
    }

    private async Task HandleMessage(RewardMessage rewardMessage)
    {
        await _emailService.NewOrderEmailAsync(rewardMessage);
    }
}