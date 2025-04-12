using Microsoft.Extensions.Options;

namespace obsidian_ai.Ollama;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddOllama(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<OllamaConfig>()
            .Bind(config.GetSection("Ollama"))
            .ValidateOnStart();
        services
            .AddSingleton<IValidateOptions<OllamaConfig>, ValidateSettingsOptions>();
        return services;
    }
}