using FluentAssertions;
using LogfileCleaner.Core;
using LogfileCleaner.Models;
using Xunit;

namespace LogfileCleaner.Tests;

public class FilterRepositoryTests : IDisposable
{
    private readonly FilterRepository _sut;
    private readonly string _testConfigPath;

    public FilterRepositoryTests()
    {
        // Use a test-specific config directory
        _testConfigPath = Path.Combine(Path.GetTempPath(), $"LogfileCleaner_Test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testConfigPath);
        Environment.SetEnvironmentVariable("APPDATA", _testConfigPath);
        
        _sut = new FilterRepository();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testConfigPath))
        {
            Directory.Delete(_testConfigPath, true);
        }
    }

    [Fact]
    public async Task AddAsync_ShouldPersistFilter()
    {
        // Arrange
        var filter = new FilterDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Test Filter",
            Description = "A test filter",
            Type = FilterType.Regex,
            Pattern = @"\d+"
        };

        // Act
        await _sut.AddAsync(filter);
        var filters = await _sut.GetAllAsync();

        // Assert
        filters.Should().ContainSingle();
        filters.First().Name.Should().Be("Test Filter");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectFilter()
    {
        // Arrange
        var filter = new FilterDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Specific Filter",
            Type = FilterType.StringContains,
            Pattern = "test"
        };
        await _sut.AddAsync(filter);

        // Act
        var result = await _sut.GetByIdAsync(filter.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Specific Filter");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveFilter()
    {
        // Arrange
        var filter = new FilterDefinition
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Type = FilterType.Regex,
            Pattern = ".*"
        };
        await _sut.AddAsync(filter);

        // Act
        await _sut.DeleteAsync(filter.Id);
        var filters = await _sut.GetAllAsync();

        // Assert
        filters.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingFilter()
    {
        // Arrange
        var filter = new FilterDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            Type = FilterType.StringContains,
            Pattern = "old"
        };
        await _sut.AddAsync(filter);

        // Act
        filter.Name = "Updated";
        filter.Pattern = "new";
        await _sut.UpdateAsync(filter);
        
        var result = await _sut.GetByIdAsync(filter.Id);

        // Assert
        result!.Name.Should().Be("Updated");
        result.Pattern.Should().Be("new");
    }
}