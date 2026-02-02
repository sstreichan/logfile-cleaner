namespace LogfileCleaner.Core;

public class LogFileReader
{
    public async Task<IEnumerable<string>> ReadLinesAsync(string filePath)
    {
        var lines = new List<string>();
        
        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            lines.Add(line);
        }
        
        return lines;
    }

    public async IAsyncEnumerable<string> StreamLinesAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            yield return line;
        }
    }
}