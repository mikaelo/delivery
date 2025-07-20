using System;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Fact]
    public void GenerateRandomLocation_WithValidCoordinates()
    {
        // Arrange
        var random = new Random(42);

        // Act
        var location = Location.CreateRandom(random);

        // Assert
        location.X.Should().BeInRange(1, 10);
        location.Y.Should().BeInRange(1, 10);
    }

    [Fact]
    public void GenerateDeterministicResults_WhenUsingSameSeed()
    {
        // Arrange
        var seed = 123;
        var random1 = new Random(seed);
        var random2 = new Random(seed);

        // Act
        var location1 = Location.CreateRandom(random1);
        var location2 = Location.CreateRandom(random2);

        // Assert
        location1.Should().Be(location2);
    }

    [Fact]
    public void GenerateDifferentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var random = new Random(42);

        // Act
        var location1 = Location.CreateRandom(random);
        var location2 = Location.CreateRandom(random);

        // Assert
        location1.Should().NotBe(location2);
    }

    [Fact]
    public void ThrowArgumentNullException_WhenRandomIsNull()
    {
        // Arrange
        Random random = null;

        // Act & Assert
        var act = () => Location.CreateRandom(random);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AlwaysGenerateValidCoordinates_WhenCalledManyTimes()
    {
        // Arrange
        var random = new Random();

        // Act & Assert
        for (int i = 0; i < 1000; i++)
        {
            var location = Location.CreateRandom(random);

            location.X.Should().BeInRange(1, 10, $"X coordinate out of bounds on iteration {i}");
            location.Y.Should().BeInRange(1, 10, $"Y coordinate out of bounds on iteration {i}");
        }
    }
}