using System;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Theory]
    [InlineData(5, 5, 3, 7)]
    [InlineData(1, 1, 10, 10)]
    [InlineData(8, 9, 1, 1)]
    public void MoveToSpecificPosition_WhenCoordinatesAreValid(
        int startX, int startY, int newX, int newY)
    {
        // Arrange
        var location = Location.Create(startX, startY);

        // Act
        var result = location.MoveTo(newX, newY);

        // Assert
        result.X.Should().Be(newX);
        result.Y.Should().Be(newY);
    }

    [Theory]
    [InlineData(5, 5, 0, 5)]  // X невалидный
    [InlineData(5, 5, 5, 0)]  // Y невалидный
    [InlineData(5, 5, 11, 5)] // X превышает максимум
    [InlineData(5, 5, 5, 11)] // Y превышает максимум
    public void FailToMoveTo_WhenCoordinatesAreInvalid(int startX, int startY, int newX, int newY)
    {
        // Arrange
        var location = Location.Create(startX, startY);

        // Act

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => location.MoveTo(newX, newY));
    }
}