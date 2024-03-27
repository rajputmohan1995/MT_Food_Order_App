namespace MT.Services.OrderAPI.RabbmitMQSender;

public interface IRabbitMQOrderMessageSender
{
    Task SendMessage(object message, string exchangeName);
}