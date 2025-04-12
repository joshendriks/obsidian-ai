using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Redis;
using StackExchange.Redis;

namespace obsidian_ai.VectorStore.Redis;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<RedisConfig>()
            .Bind(config.GetSection("Redis"))
            .ValidateOnStart();
        services
            .AddSingleton<IValidateOptions<RedisConfig>, obsidian_ai.VectorStore.Redis.ValidateSettingsOptions>();
        
        var redisConfig = config
            .GetSection("Redis")
            .Get<RedisConfig>();

        if (redisConfig != null)
        {
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
            services.AddSingleton<IConnectionMultiplexer>(multiplexer)
#pragma warning disable SKEXP0020
#pragma warning disable SKEXP0001
                .AddSingleton<IVectorStore>(_ =>
                {
                    var database = multiplexer.GetDatabase();
                    return new RedisVectorStore(database, new RedisVectorStoreOptions()
                    {
                        StorageType = RedisStorageType.HashSet
                    });
                });
#pragma warning restore SKEXP0020
#pragma warning restore SKEXP0001
        }

        return services;
    }
}