using System;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public class VolumeShould
    {
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999999)]
        public void BeCreated_WhenGivenValidValue(int validValue)
        {
            // Act
            var volume = new Volume(validValue);

            // Assert
            Assert.Equal(validValue, volume.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void ThrowArgumentException_WhenGivenInvalidValue(int invalidValue)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Volume(invalidValue));
            Assert.Contains("Объем должен быть больше 0", exception.Message);
            Assert.Equal("value", exception.ParamName);
        }
        
        [Fact]
        public void ReturnTrue_WhenCanAccommodateSmallerVolume()
        {
            // Arrange
            var currentVolume = new Volume(100);
            var otherVolume = new Volume(50);

            // Act
            var canAccommodate = currentVolume.CanAccommodate(otherVolume);

            // Assert
            Assert.True(canAccommodate);
        }

        [Fact]
        public void ReturnTrue_WhenCanAccommodateEqualVolume()
        {
            // Arrange
            var currentVolume = new Volume(100);
            var otherVolume = new Volume(100);

            // Act
            var canAccommodate = currentVolume.CanAccommodate(otherVolume);

            // Assert
            Assert.True(canAccommodate);
        }

        [Fact]
        public void ReturnFalse_WhenCannotAccommodateLargerVolume()
        {
            // Arrange
            var currentVolume = new Volume(50);
            var otherVolume = new Volume(100);

            // Act
            var canAccommodate = currentVolume.CanAccommodate(otherVolume);

            // Assert
            Assert.False(canAccommodate);
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCanAccommodateCalledWithNull()
        {
            // Arrange
            var volume = new Volume(100);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => volume.CanAccommodate(null));
            Assert.Equal("other", exception.ParamName);
        }
    }
    