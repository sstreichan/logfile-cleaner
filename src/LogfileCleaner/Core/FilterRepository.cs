using System.Text.Json;
using LogfileCleaner.Models;

namespace LogfileCleaner.Core;

public class FilterRepository
{
    private readonly string _configPath;
    private List<FilterDefinition> _filters;

    public FilterRepository()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDir = Path.Combine(appDataPath, "LogfileCleaner");
        Directory.CreateDirectory(configDir);
        
        _configPath = Path.Combine(configDir, "filters.json");
        _filters = LoadFilters();
    }

    public async Task<IEnumerable<FilterDefinition>> GetAllAsync()
    {
        return await Task.FromResult(_filters.AsEnumerable());
    }

    public async Task<FilterDefinition?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_filters.FirstOrDefault(f => f.Id == id));
    }

    public async Task AddAsync(FilterDefinition filter)
    {
        _filters.Add(filter);
        await SaveFilters();
    }

    public async Task UpdateAsync(FilterDefinition filter)
    {
        var index = _filters.FindIndex(f => f.Id == filter.Id);
        if (index >= 0)
        {
            _filters[index] = filter;
            await SaveFilters();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        _filters.RemoveAll(f => f.Id == id);
        await SaveFilters();
    }

    private List<FilterDefinition> LoadFilters()
    {
        if (!File.Exists(_configPath))
            return new List<FilterDefinition>();

        try
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<List<FilterDefinition>>(json) ?? new List<FilterDefinition>();
        }
        catch
        {
            return new List<FilterDefinition>();
        }
    }

    private async Task SaveFilters()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        var json = JsonSerializer.Serialize(_filters, options);
        await File.WriteAllTextAsync(_configPath, json);
    }
}