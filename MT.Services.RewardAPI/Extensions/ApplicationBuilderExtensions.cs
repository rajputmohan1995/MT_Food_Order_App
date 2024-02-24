using MT.Services.RewardAPI.Messaging.Interface;

namespace MT.Services.EmailAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer azureServiceBusConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        azureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopping.Register(OnStop);
        return app;
    }
    private static void OnStart()
    {
        azureServiceBusConsumer.Start();
    }
    private static void OnStop()
    {
        azureServiceBusConsumer.Stop();
    }
}