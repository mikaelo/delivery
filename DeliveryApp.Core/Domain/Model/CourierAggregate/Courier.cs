using System.Diagnostics.CodeAnalysis;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

// Главный класс курьера
    public class Courier : Aggregate<Guid>
    {
        public string Name { get; }
        public Speed Speed { get; }
        public Location Location { get; private set; }
        public List<StoragePlace> StoragePlaces { get; private set; }

        // Приватный конструктор для Entity Framework или десериализации
        [ExcludeFromCodeCoverage]
        private Courier() { }
        
        // Приватный конструктор
        private Courier(string name, Speed speed, Location location, StoragePlace storagePlace)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Speed = speed;
            Location = location;
            StoragePlaces = new List<StoragePlace>()
            {
                storagePlace
            };
        }

        // Фабричный метод для создания курьера
        public static Courier Create(string name, Speed speed, Location location)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Courier name cannot be empty", nameof(name));
            
            if (speed == null)
                throw new ArgumentNullException(nameof(speed));
            
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var defaultStoragePlace = StoragePlace.Create("Сумка", Volume.Create(10));
            return new Courier(name, speed, location, defaultStoragePlace);
        }

        // Добавить новое место хранения
        public void AddStoragePlace(string name, Volume volume)
        {
            var storagePlace = StoragePlace.Create(name, volume);
            StoragePlaces.Add(storagePlace);
        }

        // Проверить, может ли курьер взять заказ
        public bool CanTakeOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return StoragePlaces.Any(sp => sp.CanStore(order.Volume));
        }

        // Взять заказ
        public void TakeOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            
            var availableStorage = StoragePlaces.FirstOrDefault(sp => sp.CanStore(order.Volume));
            
            if (availableStorage == null)
                throw new InvalidOperationException("No available storage place for this order");
            
            availableStorage.Store(order.Id, order.Volume);
        }

        // Завершить заказ
        public void CompleteOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var storageWithOrder = StoragePlaces.FirstOrDefault(sp => sp.OrderId == order.Id);
            
            if (storageWithOrder == null)
                throw new InvalidOperationException("Order not found in courier's storage");

            storageWithOrder.Clear();
        }

        // Рассчитать количество тактов для доставки до указанной локации
        public double CalculateTimeToLocation(Location targetLocation)
        {
            if (targetLocation == null)
                throw new ArgumentNullException(nameof(targetLocation));

            int distance = Location.DistanceTo(targetLocation);
            return Math.Ceiling((double)distance / Speed.Value);
        }

        // Переместиться на один шаг к целевой локации
        public void Move(Location target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var difX = target.X - Location.X;
            var difY = target.Y - Location.Y;
            var cruisingRange = Speed.Value;

            var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
            cruisingRange -= Math.Abs(moveX);

            var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

            var newLocation = Location.Create(Location.X + moveX, Location.Y + moveY);

            Location = newLocation;
        }
        
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"Courier {Name} (ID: {Id}, Speed: {Speed}, Location: {Location})";
        }
    }