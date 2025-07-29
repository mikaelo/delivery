using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void ReturnTrue_WhenCanPlaceOrderWithSmallerVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderVolume = Volume.Create(50);

            // Act
            var canPlace = storagePlace.CanStore(orderVolume);

            // Assert
            Assert.True(canPlace);
        }

        [Fact]
        public void ReturnFalse_WhenCannotPlaceOrderWithLargerVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderVolume = Volume.Create(150);

            // Act
            var canPlace = storagePlace.CanStore(orderVolume);

            // Assert
            Assert.False(canPlace);
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCanPlaceOrderCalledWithNullVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => storagePlace.CanStore(null));
        }

        [Fact]
        public void ReturnFalse_WhenCanPlaceOrderCalledOnOccupiedStorage()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderId = Guid.NewGuid();
            storagePlace.Store(orderId, Volume.Create(50));

            // Act
            var canPlace = storagePlace.CanStore(Volume.Create(30));

            // Assert
            Assert.False(canPlace);
        }

        [Fact]
        public void PlaceOrder_WhenStorageIsEmptyAndVolumeIsAppropriate()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderId = Guid.NewGuid();
            var orderVolume = Volume.Create(50);

            // Act
            storagePlace.Store(orderId, orderVolume);

            // Assert
            Assert.Equal(orderId, storagePlace.OrderId);
        }

        [Fact]
        public void ThrowInvalidOperationException_WhenPlaceOrderWithExcessiveVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderId = Guid.NewGuid();
            var orderVolume = Volume.Create(150);

            // Act & Assert
            var exception =
                Assert.Throws<InvalidOperationException>(() => storagePlace.Store(orderId, orderVolume));
            Assert.Contains("Объем заказа (150 ед.) превышает объем места хранения (100 ед.)", exception.Message);
        }

        [Fact]
        public void ThrowArgumentNullException_WhenPlaceOrderCalledWithNullVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => storagePlace.Store(orderId, null));
        }

        [Fact]
        public void ThrowInvalidOperationException_WhenPlaceOrderInOccupiedStorage()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var firstOrderId = Guid.NewGuid();
            var secondOrderId = Guid.NewGuid();
            storagePlace.Store(firstOrderId, Volume.Create(50));

            // Act & Assert
            var exception =
                Assert.Throws<InvalidOperationException>(() => storagePlace.Store(secondOrderId, Volume.Create(30)));
            Assert.Contains("В месте хранения уже находится другой заказ", exception.Message);
        }
    }
}