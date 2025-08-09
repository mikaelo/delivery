using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

/// <summary>
///     Repository для Aggregate Courier
/// </summary>
public interface ICourierRepository : IRepository<Courier>
{
    /// <summary>
    /// Добавить курьера
    /// </summary>
    /// <param name="courier"></param>
    /// <returns></returns>
    Task AddAsync(Courier courier);
    
    /// <summary>
    /// Обновить информацию о курьере
    /// </summary>
    /// <param name="courier"></param>
    void Update(Courier courier);
    
    /// <summary>
    /// Найти курьера
    /// </summary>
    /// <param name="courierId">Идентификатор</param>
    /// <returns>Курье или None</returns>
    Task<Maybe<Courier>> GetAsync(Guid courierId);
    
    /// <summary>
    /// Найти всех свободных курьеров
    /// </summary>
    /// <returns></returns>
    IEnumerable<Courier> FindAllFree();
}