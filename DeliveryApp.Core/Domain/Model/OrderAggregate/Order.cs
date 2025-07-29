using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

    public class Order
    {
        public Guid Id { get; }
        public Location Location { get; }
        public Volume Volume { get; }
        public OrderStatus Status { get; private set; }
        public Guid? CourierId { get; private set; }

        // Приватный конструктор для Entity Framework или десериализации
        private Order() { }

        private Order(Guid orderId, Location location, Volume volume) : this()
        {
            Id = orderId;
            Location = location;
            Volume = volume;
            Status = OrderStatus.Created;
        }

        
        // Фабричный метод для создания заказа
        public static Order Create(Guid id, Location location, Volume volume)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Order ID cannot be empty", nameof(id));
            
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (volume == null)
                throw new ArgumentNullException(nameof(volume));
            
            return new Order(id, location, volume);
        }

        // Метод для назначения заказа курьеру
        public void Assign(Courier courier)
        {
            if (courier == null)
                throw new ArgumentException("Courier ID cannot be empty", nameof(courier));

            if (Status != OrderStatus.Created)
                throw new InvalidOperationException($"Cannot assign order with status {Status}. Order must be in Created status.");

            CourierId = courier.Id;
            Status = OrderStatus.Assigned;
        }

        // Метод для завершения заказа
        public void Complete()
        {
            if (Status != OrderStatus.Assigned)
                throw new InvalidOperationException($"Cannot complete order with status {Status}. Order must be assigned to a courier first.");
            
            Status = OrderStatus.Completed;
        }
        
    }