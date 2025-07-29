using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Место хранения курьера (рюкзак, багажник и т.п.)
    /// </summary>
    public class StoragePlace : Entity<Guid>
    {
        /// <summary>
        /// Название места хранения (рюкзак, багажник и т.п.)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Допустимый объем места хранения
        /// </summary>
        public Volume TotalVolume { get; }

        /// <summary>
        /// Идентификатор заказа, который хранится в месте хранения
        /// </summary>
        public Guid? OrderId { get; private set; }

        /// <summary>
        /// Проверяет, является ли место хранения занятым
        /// </summary>
        private bool IsOccupied => OrderId.HasValue;
        
        /// <summary>
        /// Для EF
        /// </summary>
        [ExcludeFromCodeCoverage]
        private StoragePlace() {}
        
        /// <summary>
        /// Приватный конструктор для создания места хранения
        /// </summary>
        /// <param name="name">Название места хранения</param>
        /// <param name="totalVolume">Допустимый объем</param>
        private StoragePlace(string name, Volume totalVolume) : this()
        {
            Id = Guid.CreateVersion7();
            Name = name;
            TotalVolume = totalVolume;
            OrderId = null;
        }
        
        /// <summary>
        /// Фабричный метод для создания нового места хранения
        /// </summary>
        /// <param name="name">Название места хранения</param>
        /// <param name="totalVolume">Допустимый объем</param>
        /// <returns>Новое место хранения</returns>
        /// <exception cref="ArgumentException">Выбрасывается при некорректных параметрах</exception>
        public static StoragePlace Create(string name, Volume totalVolume)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название места хранения не может быть пустым", nameof(name));

            if (totalVolume == null)
                throw new ArgumentNullException(nameof(totalVolume), "Объем места хранения обязателен");
                    
            return new StoragePlace(name, totalVolume);
        }
        
        /// <summary>
        /// Проверяет, можно ли поместить заказ в место хранения
        /// </summary>
        /// <param name="orderVolume">Объем заказа</param>
        /// <returns>true - если можно поместить заказ, false - если нельзя</returns>
        public bool CanStore(Volume orderVolume)
        {
            if (orderVolume == null)
                throw new ArgumentNullException(nameof(orderVolume));

            // Проверяем, что место хранения пустое
            if (IsOccupied)
                return false;

            // Проверяем, что объем места хранения может вместить заказ
            return TotalVolume.CanAccommodate(orderVolume);
        }

        /// <summary>
        /// Помещает заказ в место хранения
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="orderVolume">Объем заказа</param>
        /// <exception cref="InvalidOperationException">Выбрасывается при попытке поместить заказ в занятое место или при превышении объема</exception>
        public void Store(Guid orderId, Volume orderVolume)
        {
            if (orderVolume == null)
                throw new ArgumentNullException(nameof(orderVolume));

            if (IsOccupied)
                throw new InvalidOperationException("В месте хранения уже находится другой заказ");

            if (!TotalVolume.CanAccommodate(orderVolume))
                throw new InvalidOperationException($"Объем заказа ({orderVolume}) превышает объем места хранения ({TotalVolume})");

            OrderId = orderId;
        }

        /// <summary>
        /// Извлекает заказ из места хранения
        /// </summary>
        /// <returns>Идентификатор извлеченного заказа</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается при попытке извлечь заказ из пустого места хранения</exception>
        public void Clear()
        {
            if (!IsOccupied)    
                throw new InvalidOperationException("Место хранения пустое, нечего извлекать");
            
            OrderId = null;
        }
        
        /// <summary>
        /// Переопределение метода ToString для удобства отладки
        /// </summary>
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var status = !IsOccupied ? "пустое" : $"занято заказом {OrderId}";
            return $"StoragePlace [{Id}]: {Name} (объем: {TotalVolume}, статус: {status})";
        }
    }
}