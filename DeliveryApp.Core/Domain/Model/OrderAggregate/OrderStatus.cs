using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

/// <summary>
/// Value Object для статуса заказа
/// </summary>
public class OrderStatus : ValueObject
{
    [ExcludeFromCodeCoverage]
    private OrderStatus()
    {
    }

    /// <summary>
    ///     Cтатус заказа
    /// </summary>
    /// <param name="name">Название</param>
    private OrderStatus(string name) : this()
    {
        Name = name;
    }

    public static OrderStatus Created => new(nameof(Created).ToLowerInvariant());
    public static OrderStatus Assigned => new(nameof(Assigned).ToLowerInvariant());
    public static OrderStatus Completed => new(nameof(Completed).ToLowerInvariant());

    /// <summary>
    ///     Название
    /// </summary>
    public string Name { get; }

    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}