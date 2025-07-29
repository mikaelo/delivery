using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void WorkThroughCompleteLifecycle_WhenPlacingAndExtractingOrder()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(200));
            var orderId = Guid.NewGuid();
            var orderVolume = Volume.Create(150);

            // Act & Assert - начальное состояние
            Assert.True(storagePlace.CanStore(orderVolume));

            // Act & Assert - размещение заказа
            storagePlace.Store(orderId, orderVolume);
            Assert.Equal(orderId, storagePlace.OrderId);
            Assert.False(storagePlace.CanStore(Volume.Create(100))); // нельзя разместить другой заказ

            // Act & Assert - извлечение заказа
            storagePlace.Clear();
            Assert.True(storagePlace.CanStore(orderVolume)); // снова можно разместить заказ
        }
    }
}