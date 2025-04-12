using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace obsidian_ai.VectorStore.Redis;

public class RedisConfig
{   
    [Required]
    public string RedisServer { get; set; } = null!;
}


[OptionsValidator]
public partial class ValidateSettingsOptions : IValidateOptions<RedisConfig>
{
}