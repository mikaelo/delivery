using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Theory]
    [InlineData(1, 1, "(1, 1)")]
    [InlineData(5, 7, "(5, 7)")]
    [InlineData(10, 10, "(10, 10)")]
    public void FormatAsCoordinateString_WhenConverted(int x, int y, string expected)
    {
        // Arrange
        var location = Location.Create(x, y).Value;

        // Act
        var result = location.ToString();

        // Assert
        result.Should().Be(expected);
    }
}