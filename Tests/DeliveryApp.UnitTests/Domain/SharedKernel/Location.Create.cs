using System;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Fact]
    public void BeCreatedSuccessfully_WhenCoordinatesAreValid()
    {
        // Arrange & Act
        var result = Location.Create(5, 7);

        // Assert ;
        result.X.Should().Be(5);
        result.Y.Should().Be(7);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(5, 5)]
    public void BeCreatedSuccessfully_WhenCoordinatesAreBoundaryValid(int x, int y)
    {
        // Arrange & Act
        var result = Location.Create(x, y);

        // Assert
        result.X.Should().Be(x);
        result.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(11, 5)]
    [InlineData(5, 0)]
    [InlineData(5, 11)]
    [InlineData(-1, 5)]
    [InlineData(5, -1)]
    public void FailToCreate_WhenCoordinatesAreInvalid(int x, int y)
    {
        // Arrange & Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Location.Create(x, y));
    }
}