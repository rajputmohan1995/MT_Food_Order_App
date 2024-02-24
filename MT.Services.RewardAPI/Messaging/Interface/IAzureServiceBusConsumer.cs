namespace MT.Services.RewardAPI.Messaging.Interface;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}