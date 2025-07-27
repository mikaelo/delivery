using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void ExtractOrder_WhenStorageContainsOrder()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));
            var orderId = Guid.NewGuid();
            storagePlace.PlaceOrder(orderId, new Volume(50));

            // Act
            var extractedOrderId = storagePlace.ExtractOrder();

            // Assert
            Assert.Equal(orderId, extractedOrderId);
            Assert.Null(storagePlace.OrderId);
            Assert.True(storagePlace.IsEmpty);
        }

        [Fact]
        public void ThrowInvalidOperationException_WhenExtractOrderFromEmptyStorage()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", new Volume(100));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => storagePlace.ExtractOrder());
            Assert.Contains("Место хранения пустое, нечего извлекать", exception.Message);
        }
    }
}