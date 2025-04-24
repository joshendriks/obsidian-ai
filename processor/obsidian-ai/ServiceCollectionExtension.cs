using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace obsidian_ai;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddQueuing(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<RedisConfig>()
            .Bind(config.GetSection("Redis"))
            .ValidateOnStart();
        services
            .AddSingleton<IValidateOptions<RedisConfig>, ValidateSettingsOptions>();
        
        var redisConfig = config
            .GetSection("Redis")
            .Get<RedisConfig>();
        
        var rconfig = new ConfigurationOptions
        {
            EndPoints =
            {
                redisConfig.RedisServer
            },
            AbortOnConnectFail = false,
            ConnectRetry = 10,
            ReconnectRetryPolicy = new ExponentialRetry(5000),
            ClientName = "ApiClient"
        };
        
        var multiplexer = ConnectionMultiplexer.Connect(rconfig);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddTransient<QueueThing>();
        return services;
    }
}

public class RedisConfig
{   
    [Required]
    public string RedisServer { get; set; }
}


[OptionsValidator]
public partial class ValidateSettingsOptions : IValidateOptions<RedisConfig>
{
}