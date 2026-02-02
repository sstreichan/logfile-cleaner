using FluentAssertions;
using LogfileCleaner.Core;
using LogfileCleaner.Models;
using Xunit;

namespace LogfileCleaner.Tests;

public class FilterEngineTests
{
    private readonly FilterEngine _sut;

    public FilterEngineTests()
    {
        _sut = new FilterEngine();
    }

    [Fact]
    public void ApplyFilters_WithRegexFilter_ShouldMatchPattern()
    {
        // Arrange
        var lines = new[] { "ERROR: Something went wrong", "INFO: All good", "ERROR: Another error" };
        var filter = new FilterDefinition
        {
            Type = FilterType.Regex,
            Pattern = "^ERROR:",
            IsInverted = false
        };

        // Act
        var result = _sut.ApplyFilters(lines, new[] { filter }).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(line => line.Should().StartWith("ERROR:"));
    }

    [Fact]
    public void ApplyFilters_WithInvertedFilter_ShouldExcludeMatches()
    {
        // Arrange
        var lines = new[] { "DEBUG: Debug info", "INFO: Information", "DEBUG: More debug" };
        var filter = new FilterDefinition
        {
            Type = FilterType.StringContains,
            Pattern = "DEBUG",
            IsInverted = true
        };

        // Act
        var result = _sut.ApplyFilters(lines, new[] { filter }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Contain("INFO");
    }

    [Fact]
    public void ApplyFilters_WithLogLevelFilter_ShouldMatchMultipleLevels()
    {
        // Arrange
        var lines = new[] 
        { 
            "[INFO] Application started", 
            "[DEBUG] Detailed info",
            "[ERROR] Critical error",
            "[WARN] Warning message"
        };
        var filter = new FilterDefinition
        {
            Type = FilterType.LogLevel,
            Pattern = "ERROR,WARN",
            IsInverted = false
        };

        // Act
        var result = _sut.ApplyFilters(lines, new[] { filter }).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(line => line.Contains("ERROR"));
        result.Should().Contain(line => line.Contains("WARN"));
    }

    [Fact]
    public void ApplyFilters_WithMultipleFilters_ShouldApplyInOrder()
    {
        // Arrange
        var lines = new[] 
        { 
            "2024-01-01 ERROR: Error 1",
            "2024-01-01 INFO: Info message",
            "2024-01-02 ERROR: Error 2",
            "2024-01-02 DEBUG: Debug info"
        };
        
        var filters = new[]
        {
            new FilterDefinition { Type = FilterType.StringContains, Pattern = "2024-01-01" },
            new FilterDefinition { Type = FilterType.StringContains, Pattern = "ERROR" }
        };

        // Act
        var result = _sut.ApplyFilters(lines, filters).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be("2024-01-01 ERROR: Error 1");
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("TEST", true)]
    [InlineData("Test", true)]
    [InlineData("xyz", false)]
    public void ApplyFilters_StringContains_ShouldBeCaseInsensitive(string pattern, bool shouldMatch)
    {
        // Arrange
        var lines = new[] { "This is a test line" };
        var filter = new FilterDefinition
        {
            Type = FilterType.StringContains,
            Pattern = pattern,
            IsInverted = false
        };

        // Act
        var result = _sut.ApplyFilters(lines, new[] { filter }).ToList();

        // Assert
        if (shouldMatch)
            result.Should().HaveCount(1);
        else
            result.Should().BeEmpty();
    }
}