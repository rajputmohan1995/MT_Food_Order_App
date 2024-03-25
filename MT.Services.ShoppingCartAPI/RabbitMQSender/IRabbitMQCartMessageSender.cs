namespace MT.Services.ShoppingCartAPI.RabbitMQSender;

public interface IRabbitMQCartMessageSender
{
    Task SendMessage(object message, string queueName);
}