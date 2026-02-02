using System.Text.RegularExpressions;
using LogfileCleaner.Models;

namespace LogfileCleaner.Core;

public class FilterValidator
{
    public bool ValidatePattern(string pattern, FilterType type)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        return type switch
        {
            FilterType.Regex => ValidateRegex(pattern),
            FilterType.StringContains => true,
            FilterType.StringStartsWith => true,
            FilterType.StringEndsWith => true,
            FilterType.LogLevel => ValidateLogLevel(pattern),
            _ => false
        };
    }

    private bool ValidateRegex(string pattern)
    {
        try
        {
            _ = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidateLogLevel(string pattern)
    {
        var levels = pattern.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return levels.Length > 0 && levels.All(l => !string.IsNullOrWhiteSpace(l));
    }
}