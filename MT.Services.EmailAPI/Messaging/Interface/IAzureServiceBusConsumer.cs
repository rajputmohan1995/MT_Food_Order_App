namespace MT.Services.EmailAPI.Messaging.Interface;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}