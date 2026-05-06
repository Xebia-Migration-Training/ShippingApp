using FluentAssertions;
using Moq;
using ShippingRules.Application.Interfaces;
using ShippingRules.Application.Services;
using ShippingRules.Domain.Entities;

namespace ShippingRules.Tests.Services;

public class ExchangeRateServiceTests
{
    private readonly Mock<IExchangeRateRepository> _repoMock = new();
    private readonly ExchangeRateService _sut;

    public ExchangeRateServiceTests()
    {
        _sut = new ExchangeRateService(_repoMock.Object);
    }

    [Fact]
    public async Task TryGetRateAsync_SameCurrency_ShouldReturnOne()
    {
        // Act
        var result = await _sut.TryGetRateAsync("USD", "USD", DateTime.UtcNow);

        // Assert
        result.Should().Be(1m);
        _repoMock.Verify(r => r.GetRateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(null, "USD")]
    [InlineData("USD", null)]
    [InlineData("", "USD")]
    [InlineData("USD", "")]
    [InlineData("  ", "USD")]
    public async Task TryGetRateAsync_NullOrWhitespaceCurrency_ShouldReturnNull(string? from, string? to)
    {
        // Act
        var result = await _sut.TryGetRateAsync(from!, to!, DateTime.UtcNow);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task TryGetRateAsync_ValidPair_ShouldReturnRate()
    {
        // Arrange
        var rate = new ExchangeRate { Rate = 83m };
        _repoMock.Setup(r => r.GetRateAsync("USD", "INR", It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rate);

        // Act
        var result = await _sut.TryGetRateAsync("USD", "INR", DateTime.UtcNow);

        // Assert
        result.Should().Be(83m);
    }

    [Fact]
    public async Task TryGetRateAsync_NoRateFound_ShouldReturnNull()
    {
        // Arrange
        _repoMock.Setup(r => r.GetRateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExchangeRate?)null);

        // Act
        var result = await _sut.TryGetRateAsync("USD", "GBP", DateTime.UtcNow);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrow()
    {
        // Act & Assert
        var act = () => new ExchangeRateService(null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("repo");
    }
}
