using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void BeCreated_WhenUsingCreateFactoryMethod()
        {
            // Arrange
            var name = "Рюкзак";
            var totalVolume = new Volume(100);

            // Act
            var storagePlace = StoragePlace.Create(name, totalVolume);

            // Assert
            Assert.NotEqual(Guid.Empty, storagePlace.Id);
            Assert.Equal(name, storagePlace.Name);
            Assert.Equal(totalVolume, storagePlace.TotalVolume);
            Assert.Null(storagePlace.OrderId);
            Assert.True(storagePlace.IsEmpty);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ThrowArgumentException_WhenCreatedWithInvalidName(string invalidName)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => StoragePlace.Create(invalidName, new Volume(100)));
            Assert.Contains("Название места хранения не может быть пустым", exception.Message);
        }
        
        [Fact]
        public void ThrowArgumentNullException_WhenCreatedWithNullVolume()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => StoragePlace.Create("Рюкзак", null));
            Assert.Contains("Объем места хранения обязателен", exception.Message);
        }
        
        [Fact]
        public void NotBeEqual_WhenComparingDifferentStoragePlacesWithSameParameters()
        {
            // Arrange & Act
            var storage1 = StoragePlace.Create("Рюкзак", new Volume(100));
            var storage2 = StoragePlace.Create("Рюкзак", new Volume(100));

            // Assert - объекты должны быть разными, даже при одинаковых параметрах
            Assert.NotEqual(storage1.Id, storage2.Id);
            Assert.Equal(storage1.Name, storage2.Name); // название должно быть одинаковым
            Assert.Equal(storage1.TotalVolume.Value, storage2.TotalVolume.Value); // объем должен быть одинаковым
            
            // Но сами объекты должны быть разными (если переопределен Equals)
            Assert.False(ReferenceEquals(storage1, storage2));
        }

    }
}