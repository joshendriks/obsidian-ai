using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using obsidian_ai;
using Testcontainers.Redis;

namespace Integration;

public sealed class CustomWebApplicationFactory(
    RedisContainer redisContainer)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var conn = redisContainer.GetConnectionString();
        builder.UseEnvironment("Integration")
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new("Redis:RedisServer", redisContainer.GetConnectionString())
                }!);
            })
            .ConfigureTestServices(services =>
            {

            });
    }
}