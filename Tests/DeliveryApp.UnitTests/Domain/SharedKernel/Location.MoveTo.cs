using DeliveryApp.Core.Domain.SharedKernel;
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
        var location = Location.Create(startX, startY).Value;

        // Act
        var result = location.MoveTo(newX, newY);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(newX);
        result.Value.Y.Should().Be(newY);
    }

    [Theory]
    [InlineData(5, 5, 0, 5)]  // X невалидный
    [InlineData(5, 5, 5, 0)]  // Y невалидный
    [InlineData(5, 5, 11, 5)] // X превышает максимум
    [InlineData(5, 5, 5, 11)] // Y превышает максимум
    public void FailToMoveTo_WhenCoordinatesAreInvalid(int startX, int startY, int newX, int newY)
    {
        // Arrange
        var location = Location.Create(startX, startY).Value;

        // Act
        var result = location.MoveTo(newX, newY);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeEmpty();
    }
}