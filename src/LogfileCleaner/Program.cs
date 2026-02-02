using Spectre.Console;
using LogfileCleaner.Core;
using LogfileCleaner.Models;

namespace LogfileCleaner;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var logo = new FigletText("LogCleaner")
            .Centered()
            .Color(Color.Blue);
        
        AnsiConsole.Write(logo);
        AnsiConsole.MarkupLine("[dim]Clean your logfiles with style[/]\n");

        var filterRepo = new FilterRepository();
        
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]What would you like to do?[/]")
                    .AddChoices(new[] {
                        "Clean a logfile",
                        "Manage filters",
                        "Exit"
                    }));

            switch (choice)
            {
                case "Clean a logfile":
                    await CleanLogfile(filterRepo);
                    break;
                case "Manage filters":
                    await ManageFilters(filterRepo);
                    break;
                case "Exit":
                    AnsiConsole.MarkupLine("[blue]Goodbye![/]");
                    return 0;
            }
        }
    }

    static async Task CleanLogfile(FilterRepository filterRepo)
    {
        var filePath = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Enter logfile path:[/]")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Invalid file path[/]")
                .Validate(path =>
                {
                    if (string.IsNullOrWhiteSpace(path))
                        return ValidationResult.Error("[red]Path cannot be empty[/]");
                    if (!File.Exists(path))
                        return ValidationResult.Error("[red]File does not exist[/]");
                    return ValidationResult.Success();
                }));

        var filters = await filterRepo.GetAllAsync();
        
        if (!filters.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No filters available. Create one first![/]");
            return;
        }

        var selectedFilters = AnsiConsole.Prompt(
            new MultiSelectionPrompt<FilterDefinition>()
                .Title("[green]Select filters to apply:[/]")
                .NotRequired()
                .PageSize(10)
                .AddChoices(filters)
                .UseConverter(f => $"{f.Name} - {f.Description}"));

        if (!selectedFilters.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No filters selected.[/]");
            return;
        }

        var outputPath = Path.Combine(
            Path.GetDirectoryName(filePath) ?? ".",
            $"{Path.GetFileNameWithoutExtension(filePath)}_cleaned{Path.GetExtension(filePath)}");

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[green]Processing logfile...[/]");
                
                var reader = new LogFileReader();
                var engine = new FilterEngine();
                
                var lines = await reader.ReadLinesAsync(filePath);
                task.Increment(30);
                
                var filtered = engine.ApplyFilters(lines, selectedFilters);
                task.Increment(40);
                
                await File.WriteAllLinesAsync(outputPath, filtered);
                task.Increment(30);
            });

        AnsiConsole.MarkupLine($"[green]✓[/] Cleaned logfile saved to: [blue]{outputPath}[/]");
    }

    static async Task ManageFilters(FilterRepository filterRepo)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Filter Management[/]")
                .AddChoices(new[] {
                    "Create new filter",
                    "List filters",
                    "Delete filter",
                    "Back"
                }));

        switch (choice)
        {
            case "Create new filter":
                await CreateFilter(filterRepo);
                break;
            case "List filters":
                await ListFilters(filterRepo);
                break;
            case "Delete filter":
                await DeleteFilter(filterRepo);
                break;
        }
    }

    static async Task CreateFilter(FilterRepository filterRepo)
    {
        var name = AnsiConsole.Ask<string>("[yellow]Filter name:[/]");
        var description = AnsiConsole.Ask<string>("[yellow]Description:[/]");
        
        var filterType = AnsiConsole.Prompt(
            new SelectionPrompt<FilterType>()
                .Title("[green]Filter type:[/]")
                .AddChoices(Enum.GetValues<FilterType>()));

        var pattern = AnsiConsole.Ask<string>("[yellow]Pattern (regex or string):[/]");

        var validator = new FilterValidator();
        if (!validator.ValidatePattern(pattern, filterType))
        {
            AnsiConsole.MarkupLine("[red]Invalid pattern![/]");
            return;
        }

        var filter = new FilterDefinition
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Type = filterType,
            Pattern = pattern,
            IsInverted = AnsiConsole.Confirm("Invert filter (exclude matches)?", false)
        };

        await filterRepo.AddAsync(filter);
        AnsiConsole.MarkupLine("[green]✓ Filter created successfully![/]");
    }

    static async Task ListFilters(FilterRepository filterRepo)
    {
        var filters = await filterRepo.GetAllAsync();
        
        if (!filters.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No filters found.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Type");
        table.AddColumn("Pattern");
        table.AddColumn("Inverted");

        foreach (var filter in filters)
        {
            table.AddRow(
                filter.Name,
                filter.Type.ToString(),
                filter.Pattern,
                filter.IsInverted ? "Yes" : "No"
            );
        }

        AnsiConsole.Write(table);
    }

    static async Task DeleteFilter(FilterRepository filterRepo)
    {
        var filters = await filterRepo.GetAllAsync();
        
        if (!filters.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No filters to delete.[/]");
            return;
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<FilterDefinition>()
                .Title("[red]Select filter to delete:[/]")
                .AddChoices(filters)
                .UseConverter(f => f.Name));

        if (AnsiConsole.Confirm($"Delete '{selected.Name}'?"))
        {
            await filterRepo.DeleteAsync(selected.Id);
            AnsiConsole.MarkupLine("[green]✓ Filter deleted![/]");
        }
    }
}