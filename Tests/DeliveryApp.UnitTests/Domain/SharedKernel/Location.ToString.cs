using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public partial class LocationShould
{
    [Theory]
    [InlineData(1, 1, "(X:1, Y:1)")]
    [InlineData(5, 7, "(X:5, Y:7)")]
    [InlineData(10, 10, "(X:10, Y:10)")]
    public void FormatAsCoordinateString_WhenConverted(int x, int y, string expected)
    {
        // Arrange
        var location = Location.Create(x, y);

        // Act
        var result = location.ToString();

        // Assert
        result.Should().Be(expected);
    }
}