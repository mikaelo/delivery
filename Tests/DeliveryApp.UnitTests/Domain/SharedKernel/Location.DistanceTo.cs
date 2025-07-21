using System;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Theory]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 2, 1, 1)]
    [InlineData(1, 1, 1, 2, 1)]
    [InlineData(1, 1, 2, 2, 2)]
    [InlineData(1, 1, 5, 7, 10)]
    [InlineData(5, 5, 1, 1, 8)]
    [InlineData(10, 10, 1, 1, 18)]
    public void CalculateCorrectManhattanDistance_BetweenTwoLocations(
        int x1, int y1, int x2, int y2, int expectedDistance)
    {
        // Arrange
        var location1 = Location.Create(x1, y1).Value;
        var location2 = Location.Create(x2, y2).Value;

        // Act
        var distance = location1.DistanceTo(location2);

        // Assert
        distance.Should().Be(expectedDistance);
    }

    [Fact]
    public void CalculateSymmetricDistance_InBothDirections()
    {
        // Arrange
        var location1 = Location.Create(3, 4).Value;
        var location2 = Location.Create(7, 8).Value;

        // Act
        var distance1to2 = location1.DistanceTo(location2);
        var distance2to1 = location2.DistanceTo(location1);

        // Assert
        distance1to2.Should().Be(distance2to1);
    }

    [Fact]
    public void ThrowArgumentNullException_WhenCalculatingDistanceToNull()
    {
        // Arrange
        var location = Location.Create(5, 5).Value;

        // Act & Assert
        var act = () => location.DistanceTo(null);
        act.Should().Throw<ArgumentNullException>();
    }
}