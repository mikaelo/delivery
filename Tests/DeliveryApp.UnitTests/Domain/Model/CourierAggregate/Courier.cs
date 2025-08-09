using System;
using System.Linq;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierShould
    {
        private readonly Location _minLocation = Location.MinCoordinates;
        
        [Fact]
        public void BeCreatedWithValidParameters()
        {
            // Arrange
            var name = "Алексей";
            var speed = Speed.Create(2);
            var location = Location.Create(1, 1);

            // Act
            var courier = Courier.Create(name, speed, location);

            // Assert
            Assert.NotNull(courier);
            Assert.NotEqual(Guid.Empty, courier.Id);
            Assert.Equal(name, courier.Name);
            Assert.Equal(speed, courier.Speed);
            Assert.Equal(location.X, courier.Location.X);
            Assert.Equal(location.Y, courier.Location.Y);
        }

        [Fact]
        public void HaveDefaultBagStorageWhenCreated()
        {
            // Arrange & Act
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);

            // Assert
            Assert.Single(courier.StoragePlaces);
            var bag = courier.StoragePlaces.First();
            Assert.Equal("Сумка", bag.Name);
            Assert.Equal(Volume.Create(10), bag.TotalVolume);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ThrowArgumentExceptionWhenCreatedWithInvalidName(string invalidName)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Courier.Create(invalidName, Speed.Create(1), _minLocation));
        }
        
        [Fact]
        public void ThrowArgumentNullExceptionWhenCreatedWithNullLocation()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                Courier.Create("Тест", Speed.Create(1), null));
        }
        
        [Fact]
        public void ThrowArgumentNullExceptionWhenCreatedWithNullSpeed()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                Courier.Create("Тест", null, _minLocation));
        }

        [Fact]
        public void AddNewStoragePlaceSuccessfully()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var storageName = "Термосумка";
            var volume = Volume.Create(5);

            // Act
            courier.AddStoragePlace(storageName, volume);

            // Assert
            Assert.Equal(2, courier.StoragePlaces.Count);
            var thermalBag = courier.StoragePlaces.Last();
            Assert.Equal(storageName, thermalBag.Name);
            Assert.Equal(volume, thermalBag.TotalVolume);
        }

        [Fact]
        public void ReturnTrueWhenCanTakeOrderWithAvailableStorage()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(5));

            // Act
            var canTake = courier.CanTakeOrder(order);

            // Assert
            Assert.True(canTake);
        }

        [Fact]
        public void ReturnFalseWhenCannotTakeOrderTooLarge()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var largeOrder = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(20));

            // Act
            var canTake = courier.CanTakeOrder(largeOrder);

            // Assert
            Assert.False(canTake);
        }

        [Fact]
        public void ReturnFalseWhenCannotTakeOrderNoAvailableStorage()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var order1 = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(10));
            var order2 = Order.Create(Guid.NewGuid(), Location.Create(3, 3), Volume.Create(5));
            courier.TakeOrder(order1);

            // Act
            var canTake = courier.CanTakeOrder(order2);

            // Assert
            Assert.False(canTake);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenCanTakeOrderWithNullOrder()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => courier.CanTakeOrder(null));
        }

        [Fact]
        public void TakeOrderSuccessfullyWhenStorageAvailable()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(8));

            // Act
            courier.TakeOrder(order);

            // Assert
            var storage = courier.StoragePlaces.First();
            Assert.Equal(order.Id, storage.OrderId);
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenTakeOrderWithNoStorage()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var order1 = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(10));
            var order2 = Order.Create(Guid.NewGuid(), Location.Create(3, 3), Volume.Create(5));
            courier.TakeOrder(order1);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => courier.TakeOrder(order2));
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenTakeOrderTooLarge()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var largeOrder = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(20));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => courier.TakeOrder(largeOrder));
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenTakeOrderWithNullOrder()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => courier.TakeOrder(null));
        }

        [Fact]
        public void CompleteOrderSuccessfully()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(8));
            courier.TakeOrder(order);

            // Act
            courier.CompleteOrder(order);

            // Assert
            var storage = courier.StoragePlaces.First();
            Assert.Null(storage.OrderId);
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenCompleteOrderNotFound()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5), Volume.Create(8));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => courier.CompleteOrder(order));
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenCompleteOrderWithNullOrder()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => courier.CompleteOrder(null));
        }

        [Fact]
        public void CalculateCorrectTimeToLocation()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(2), _minLocation);
            var targetLocation = Location.Create(5, 5);

            // Act
            var time = courier.CalculateTimeToLocation(targetLocation);

            // Assert
            // Расстояние: |5-1| + |5-1| = 8, скорость: 2, время: 8/2 = 4
            Assert.Equal(4, time);
        }

        [Fact]
        public void CalculateTimeToLocationWithCeilingForOddDistance()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(2), _minLocation);
            var targetLocation = Location.Create(4, 1);

            // Act
            var time = courier.CalculateTimeToLocation(targetLocation);

            // Assert
            // Расстояние: 3, скорость: 2, время: ceil(3/2) = 2
            Assert.Equal(2, time);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenCalculateTimeWithNullLocation()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => courier.CalculateTimeToLocation(null));
        }

        [Fact]
        public void MoveTowardsTargetLocationCorrectly()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(2), _minLocation);
            var targetLocation = Location.Create(6, 6);

            // Act
            courier.Move(targetLocation);

            // Assert
            // При скорости 2: движение на 2 по X, затем на 2 по Y
            Assert.Equal(3, courier.Location.X); // 1 + 2
            Assert.Equal(1, courier.Location.Y); // 1 + 2
        }

        [Fact]
        public void MoveOnlyInXDirectionWhenSpeedIsOne()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);
            var targetLocation = Location.Create(5, 5);

            // Act
            courier.Move(targetLocation);

            // Assert
            // При скорости 1 движется только по одной оси
            Assert.Equal(2, courier.Location.X); // 1 + 1
            Assert.Equal(1, courier.Location.Y); // без изменений
        }

        [Fact]
        public void MoveToExactLocationWhenCloseToTarget()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(5), Location.Create(3, 3));
            var targetLocation = Location.Create(4, 4);

            // Act
            courier.Move(targetLocation);

            // Assert
            // Должен достичь точно целевой локации
            Assert.Equal(4, courier.Location.X);
            Assert.Equal(4, courier.Location.Y);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenMoveWithNullLocation()
        {
            // Arrange
            var courier = Courier.Create("Тест", Speed.Create(1), _minLocation);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => courier.Move(null));
        }
    }
}