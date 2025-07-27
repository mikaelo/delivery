using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void ReturnTrue_WhenCanPlaceOrderWithSmallerVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderVolume = new Volume(50);

            // Act
            var canPlace = storagePlace.CanPlaceOrder(orderVolume);

            // Assert
            Assert.True(canPlace);
        }

        [Fact]
        public void ReturnFalse_WhenCannotPlaceOrderWithLargerVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderVolume = new Volume(150);

            // Act
            var canPlace = storagePlace.CanPlaceOrder(orderVolume);

            // Assert
            Assert.False(canPlace);
        }

        [Fact]
        public void ThrowArgumentNullException_WhenCanPlaceOrderCalledWithNullVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => storagePlace.CanPlaceOrder(null));
        }

        [Fact]
        public void ReturnFalse_WhenCanPlaceOrderCalledOnOccupiedStorage()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderId = Guid.NewGuid();
            storagePlace.PlaceOrder(orderId, new Volume(50));

            // Act
            var canPlace = storagePlace.CanPlaceOrder(new Volume(30));

            // Assert
            Assert.False(canPlace);
        }

        [Fact]
        public void PlaceOrder_WhenStorageIsEmptyAndVolumeIsAppropriate()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderId = Guid.NewGuid();
            var orderVolume = new Volume(50);

            // Act
            storagePlace.PlaceOrder(orderId, orderVolume);

            // Assert
            Assert.Equal(orderId, storagePlace.OrderId);
            Assert.False(storagePlace.IsEmpty);
        }

        [Fact]
        public void ThrowInvalidOperationException_WhenPlaceOrderWithExcessiveVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderId = Guid.NewGuid();
            var orderVolume = new Volume(150);

            // Act & Assert
            var exception =
                Assert.Throws<InvalidOperationException>(() => storagePlace.PlaceOrder(orderId, orderVolume));
            Assert.Contains("Объем заказа (150 ед.) превышает объем места хранения (100 ед.)", exception.Message);
        }

        [Fact]
        public void ThrowArgumentNullException_WhenPlaceOrderCalledWithNullVolume()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => storagePlace.PlaceOrder(orderId, null));
        }

        [Fact]
        public void ThrowInvalidOperationException_WhenPlaceOrderInOccupiedStorage()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var firstOrderId = Guid.NewGuid();
            var secondOrderId = Guid.NewGuid();
            storagePlace.PlaceOrder(firstOrderId, new Volume(50));

            // Act & Assert
            var exception =
                Assert.Throws<InvalidOperationException>(() => storagePlace.PlaceOrder(secondOrderId, new Volume(30)));
            Assert.Contains("В месте хранения уже находится другой заказ", exception.Message);
        }
    }
}