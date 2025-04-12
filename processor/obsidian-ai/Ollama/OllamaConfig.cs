using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace obsidian_ai.Ollama;

public class OllamaConfig
{   
    [Required]
    public string Url { get; set; } = null!;
}


[OptionsValidator]
public partial class ValidateSettingsOptions : IValidateOptions<OllamaConfig>
{
}