using System.Text.Json.Serialization;

namespace LogfileCleaner.Models;

public class FilterDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FilterType Type { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public bool IsInverted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FilterType
{
    Regex,
    StringContains,
    StringStartsWith,
    StringEndsWith,
    LogLevel
}