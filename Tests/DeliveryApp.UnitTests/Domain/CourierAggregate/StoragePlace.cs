using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void ReturnTrue_WhenIsEmptyCalledOnStorageWithoutOrder()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));

            // Act & Assert
            Assert.True(storagePlace.IsEmpty);
        }

        [Fact]
        public void ReturnFalse_WhenIsEmptyCalledOnStorageWithOrder()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderId = Guid.NewGuid();
            storagePlace.PlaceOrder(orderId, new Volume(50));

            // Act & Assert
            Assert.False(storagePlace.IsEmpty);
        }

        [Fact]
        public void WorkThroughCompleteLifecycle_WhenPlacingAndExtractingOrder()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(200));
            var orderId = Guid.NewGuid();
            var orderVolume = new Volume(150);

            // Act & Assert - начальное состояние
            Assert.True(storagePlace.IsEmpty);
            Assert.True(storagePlace.CanPlaceOrder(orderVolume));

            // Act & Assert - размещение заказа
            storagePlace.PlaceOrder(orderId, orderVolume);
            Assert.False(storagePlace.IsEmpty);
            Assert.Equal(orderId, storagePlace.OrderId);
            Assert.False(storagePlace.CanPlaceOrder(new Volume(100))); // нельзя разместить другой заказ

            // Act & Assert - извлечение заказа
            var extractedOrderId = storagePlace.ExtractOrder();
            Assert.Equal(orderId, extractedOrderId);
            Assert.True(storagePlace.IsEmpty);
            Assert.True(storagePlace.CanPlaceOrder(orderVolume)); // снова можно разместить заказ
        }
    }
}