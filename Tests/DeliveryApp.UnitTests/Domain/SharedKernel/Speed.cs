using System;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

public class SpeedShould
    {
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999999)]
        public void BeCreated_WhenGivenValidValue(int validValue)
        {
            // Act
            var speed = Speed.Create(validValue);

            // Assert
            Assert.Equal(validValue, speed.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void ThrowArgumentException_WhenGivenInvalidValue(int invalidValue)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Speed.Create(invalidValue));
            Assert.Equal("value", exception.ParamName);
        }
    }
    