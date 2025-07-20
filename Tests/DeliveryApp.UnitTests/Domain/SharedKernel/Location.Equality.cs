using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Fact]
    public void BeEqual_WhenCoordinatesAreIdentical()
    {
        // Arrange
        var location1 = Location.Create(5, 7).Value;
        var location2 = Location.Create(5, 7).Value;

        // Act & Assert
        location1.Equals(location2).Should().BeTrue();
        (location1 == location2).Should().BeTrue();
        (location1 != location2).Should().BeFalse();
    }

    [Fact]
    public void NotBeEqual_WhenCoordinatesDiffer()
    {
        // Arrange
        var location1 = Location.Create(5, 7).Value;
        var location2 = Location.Create(5, 8).Value;
        var location3 = Location.Create(6, 7).Value;

        // Act & Assert
        location1.Equals(location2).Should().BeFalse();
        location1.Equals(location3).Should().BeFalse();
        (location1 == location2).Should().BeFalse();
        (location1 != location2).Should().BeTrue();
    }
}