using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public partial class StoragePlaceShould
    {
        [Fact]
        public void ExtractOrder_WhenStorageContainsOrder()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));
            var orderId = Guid.NewGuid();
            storagePlace.Store(orderId, Volume.Create(50));

            // Act
            storagePlace.Clear();

            // Assert
            Assert.Null(storagePlace.OrderId);
        }

        [Fact]
        public void ThrowInvalidOperationException_WhenExtractOrderFromEmptyStorage()
        {
            // Arrange
            var storagePlace = StoragePlace.Create("Рюкзак", Volume.Create(100));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => storagePlace.Clear());
            Assert.Contains("Место хранения пустое, нечего извлекать", exception.Message);
        }
    }
}