using System;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Theory]
    [InlineData(5, 5, 1, 1, 6, 6)]
    [InlineData(5, 5, -1, -1, 4, 4)]
    [InlineData(1, 1, 0, 0, 1, 1)]
    [InlineData(3, 7, 2, -3, 5, 4)]
    public void MoveToNewPosition_WhenDeltaIsValid(
        int startX, int startY, int deltaX, int deltaY, int expectedX, int expectedY)
    {
        // Arrange
        var location = Location.Create(startX, startY);

        // Act
        var result = location.Move(deltaX, deltaY);

        // Assert
        result.X.Should().Be(expectedX);
        result.Y.Should().Be(expectedY);
    }

    [Theory]
    [InlineData(1, 1, -1, 0)] // X становится 0
    [InlineData(1, 1, 0, -1)] // Y становится 0
    [InlineData(10, 10, 1, 0)] // X становится 11
    [InlineData(10, 10, 0, 1)] // Y становится 11
    [InlineData(5, 5, -10, 0)] // X становится -5
    [InlineData(5, 5, 0, 10)] // Y становится 15
    public void FailToMove_WhenDeltaResultsInInvalidCoordinates(int startX, int startY, int deltaX, int deltaY)
    {
        // Arrange
        var location = Location.Create(startX, startY);

        // Act
        
        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => location.Move(deltaX, deltaY));
    }
}