using DeliveryApp.Core.Domain.SharedKernel;
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

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(5);
        result.Value.Y.Should().Be(7);
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
        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(x);
        result.Value.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(0, 5, "Координата X должна быть в диапазоне от 1 до 10. Получено: 0")]
    [InlineData(11, 5, "Координата X должна быть в диапазоне от 1 до 10. Получено: 11")]
    [InlineData(5, 0, "Координата Y должна быть в диапазоне от 1 до 10. Получено: 0")]
    [InlineData(5, 11, "Координата Y должна быть в диапазоне от 1 до 10. Получено: 11")]
    [InlineData(-1, 5, "Координата X должна быть в диапазоне от 1 до 10. Получено: -1")]
    [InlineData(5, -1, "Координата Y должна быть в диапазоне от 1 до 10. Получено: -1")]
    public void FailToCreate_WhenCoordinatesAreInvalid(int x, int y, string expectedError)
    {
        // Arrange & Act
        var result = Location.Create(x, y);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);
    }
}