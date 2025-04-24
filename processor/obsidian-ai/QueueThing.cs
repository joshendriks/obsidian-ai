using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace obsidian_ai;

public class QueueThing
{
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _options;

    const string QueueName = "myqueue";

    public QueueThing(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _options = new JsonSerializerOptions(JsonSerializerOptions.Default);
        _options.Converters.Add(new JsonStringEnumConverter());
    }
    
    public void Enqueue(DocumentChanged documentChanged)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(documentChanged, _options);
        db.ListLeftPush(QueueName, json);
    }

    // public void Dequeue()
    // {
    //     var db = redis.GetDatabase();
    //     db.Pop();
    // }
}

public record DocumentChanged(string Path, ChangeType ChangeType);

public enum ChangeType
{
    Updated,
    Deleted
}