using FluentAssertions;
using ShippingRules.Application.Services;
using ShippingRules.Domain.Entities;
using ShippingRules.Domain.Enums;

namespace ShippingRules.Tests.Services;

public class RulePrecedenceServiceTests
{
    [Fact]
    public void CalculateCost_ShouldApplyBaseRateAndSurcharge()
    {
        // Arrange
        var rule = new ShippingRule
        {
            BaseRate = 100m,
            SurchargePercentage = 10m
        };
        var service = new RulePrecedenceService(
            new Moq.Mock<Application.Interfaces.IShippingRuleRepository>().Object);

        // Act
        var cost = service.CalculateCost(rule, 500m);

        // Assert
        cost.Should().Be(150m); // 100 + (500 * 10 / 100)
    }

    [Fact]
    public void CalculateCost_WithZeroSurcharge_ShouldReturnBaseRateOnly()
    {
        // Arrange
        var rule = new ShippingRule
        {
            BaseRate = 250m,
            SurchargePercentage = 0m
        };
        var service = new RulePrecedenceService(
            new Moq.Mock<Application.Interfaces.IShippingRuleRepository>().Object);

        // Act
        var cost = service.CalculateCost(rule, 1000m);

        // Assert
        cost.Should().Be(250m);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrow()
    {
        // Act & Assert
        var act = () => new RulePrecedenceService(null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("repository");
    }
}
