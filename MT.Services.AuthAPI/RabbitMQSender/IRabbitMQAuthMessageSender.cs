namespace MT.Services.AuthAPI.RabbmitMQSender;

public interface IRabbitMQAuthMessageSender
{
    Task SendMessage(object message, string queueName);
}