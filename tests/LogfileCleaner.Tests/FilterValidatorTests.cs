using FluentAssertions;
using LogfileCleaner.Core;
using LogfileCleaner.Models;
using Xunit;

namespace LogfileCleaner.Tests;

public class FilterValidatorTests
{
    private readonly FilterValidator _sut;

    public FilterValidatorTests()
    {
        _sut = new FilterValidator();
    }

    [Theory]
    [InlineData(@"^\d{4}-\d{2}-\d{2}", true)]  // Valid regex
    [InlineData(@"[a-zA-Z]+", true)]             // Valid regex
    [InlineData(@"(", false)]                    // Invalid regex - unclosed group
    [InlineData(@"[z-a]", false)]                // Invalid regex - invalid range
    public void ValidatePattern_WithRegex_ShouldValidateCorrectly(string pattern, bool isValid)
    {
        // Act
        var result = _sut.ValidatePattern(pattern, FilterType.Regex);

        // Assert
        result.Should().Be(isValid);
    }

    [Theory]
    [InlineData("ERROR,WARN,INFO", true)]
    [InlineData("DEBUG", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("ERROR,,WARN", true)]  // Empty entry is filtered out
    public void ValidatePattern_WithLogLevel_ShouldValidateCorrectly(string pattern, bool isValid)
    {
        // Act
        var result = _sut.ValidatePattern(pattern, FilterType.LogLevel);

        // Assert
        result.Should().Be(isValid);
    }

    [Fact]
    public void ValidatePattern_WithStringContains_ShouldAlwaysReturnTrue()
    {
        // Act
        var result = _sut.ValidatePattern("any string", FilterType.StringContains);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidatePattern_WithEmptyPattern_ShouldReturnFalse()
    {
        // Act
        var result = _sut.ValidatePattern("", FilterType.StringContains);

        // Assert
        result.Should().BeFalse();
    }
}