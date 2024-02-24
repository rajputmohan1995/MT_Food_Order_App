using Azure.Messaging.ServiceBus;
using MT.Services.RewardAPI.Message;
using MT.Services.RewardAPI.Messaging.Interface;
using MT.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace MT.Services.RewardAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string newOrderTopic;
    private readonly string newOrderRewardSubscription;
    private readonly IConfiguration _configuration;
    private readonly RewardService _rewardService;

    private ServiceBusProcessor _rewardProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
    {
        _configuration = configuration;
        _rewardService = rewardService;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        newOrderTopic = _configuration.GetValue<string>("TopicAndQueueNames:NewOrderGeneratedTopic");
        newOrderRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:NewOrderRewardsUpdate_Topic_Subscription");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _rewardProcessor = client.CreateProcessor(newOrderTopic, newOrderRewardSubscription);
    }

    public async Task Start()
    {
        _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardRequestReceived;
        _rewardProcessor.ProcessErrorAsync += OnErrorHandler;
        await _rewardProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _rewardProcessor.StopProcessingAsync();
        await _rewardProcessor.DisposeAsync();
    }

    private async Task OnNewOrderRewardRequestReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        try
        {
            RewardMessage objRewardMsg = JsonConvert.DeserializeObject<RewardMessage>(body);
            await _rewardService.UpdateRewards(objRewardMsg);
            await args.CompleteMessageAsync(args.Message);
        }
        catch
        {
            throw;
        }
    }

    private Task OnErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}