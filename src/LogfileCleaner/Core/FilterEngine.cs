using System.Text.RegularExpressions;
using LogfileCleaner.Models;

namespace LogfileCleaner.Core;

public class FilterEngine
{
    public IEnumerable<string> ApplyFilters(IEnumerable<string> lines, IEnumerable<FilterDefinition> filters)
    {
        var result = lines;
        
        foreach (var filter in filters)
        {
            result = ApplyFilter(result, filter);
        }
        
        return result;
    }

    private IEnumerable<string> ApplyFilter(IEnumerable<string> lines, FilterDefinition filter)
    {
        return filter.Type switch
        {
            FilterType.Regex => FilterByRegex(lines, filter),
            FilterType.StringContains => FilterByStringContains(lines, filter),
            FilterType.StringStartsWith => FilterByStringStartsWith(lines, filter),
            FilterType.StringEndsWith => FilterByStringEndsWith(lines, filter),
            FilterType.LogLevel => FilterByLogLevel(lines, filter),
            _ => lines
        };
    }

    private IEnumerable<string> FilterByRegex(IEnumerable<string> lines, FilterDefinition filter)
    {
        var regex = new Regex(filter.Pattern, RegexOptions.Compiled);
        
        foreach (var line in lines)
        {
            var matches = regex.IsMatch(line);
            if (filter.IsInverted ? !matches : matches)
            {
                yield return line;
            }
        }
    }

    private IEnumerable<string> FilterByStringContains(IEnumerable<string> lines, FilterDefinition filter)
    {
        foreach (var line in lines)
        {
            var contains = line.Contains(filter.Pattern, StringComparison.OrdinalIgnoreCase);
            if (filter.IsInverted ? !contains : contains)
            {
                yield return line;
            }
        }
    }

    private IEnumerable<string> FilterByStringStartsWith(IEnumerable<string> lines, FilterDefinition filter)
    {
        foreach (var line in lines)
        {
            var startsWith = line.StartsWith(filter.Pattern, StringComparison.OrdinalIgnoreCase);
            if (filter.IsInverted ? !startsWith : startsWith)
            {
                yield return line;
            }
        }
    }

    private IEnumerable<string> FilterByStringEndsWith(IEnumerable<string> lines, FilterDefinition filter)
    {
        foreach (var line in lines)
        {
            var endsWith = line.EndsWith(filter.Pattern, StringComparison.OrdinalIgnoreCase);
            if (filter.IsInverted ? !endsWith : endsWith)
            {
                yield return line;
            }
        }
    }

    private IEnumerable<string> FilterByLogLevel(IEnumerable<string> lines, FilterDefinition filter)
    {
        var levels = filter.Pattern.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        foreach (var line in lines)
        {
            var hasLevel = levels.Any(level => line.Contains(level, StringComparison.OrdinalIgnoreCase));
            if (filter.IsInverted ? !hasLevel : hasLevel)
            {
                yield return line;
            }
        }
    }
}