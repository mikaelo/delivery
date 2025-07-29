using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

/// <summary>
/// Value Object для представления скорости
/// </summary>
public class Speed : ValueObject
{
    /// <summary>
    /// Значение скорости
    /// </summary>
    public int Value { get; }
    
    /// <summary>
    /// Конструктор для создания значения скорости
    /// </summary>
    /// <param name="value">Значение скорости</param>
    private Speed(int value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Создает новое значение скорости
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>Speed</returns> 
    public static Speed Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Скорость должен быть больше 0", nameof(value));
        
        return new Speed(value);
    }
    
    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    /// <summary>
    /// Строковое представление скорости
    /// </summary>
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"{Value}";
    }
}