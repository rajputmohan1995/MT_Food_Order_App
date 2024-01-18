using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace MT.MessageBus;

public class MessageBus : IMessageBus
{
    private string connectionString = "Endpoint=sb://mt-web.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=U2pDH9c8PROB4utDH5nZzJ/JepcHiOrAb+ASbLmrQo4=;EntityPath=emailshoppingcart";
    public async Task PublishMessage(object message, string topic_queue_name)
    {
        await using var client = new ServiceBusClient(connectionString);

        ServiceBusSender sender = client.CreateSender(topic_queue_name);

        var jsonMsg = JsonConvert.SerializeObject(message);
        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMsg))
        {
            CorrelationId = Guid.NewGuid().ToString(),
        };

        await sender.SendMessageAsync(finalMessage);
        await client.DisposeAsync();
    }
}