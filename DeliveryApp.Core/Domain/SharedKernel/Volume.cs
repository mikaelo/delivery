﻿using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

/// <summary>
/// Value Object для представления объема
/// </summary>
public class Volume : ValueObject
{
    /// <summary>
    /// Значение объема
    /// </summary>
    public int Value { get; }
    
    /// <summary>
    /// Конструктор для создания значения объема
    /// </summary>
    /// <param name="value">Значение объема</param>
    /// <exception cref="ArgumentException">Выбрасывается если объем меньше или равен 0</exception>
    private Volume(int value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Создает новое значение объема
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>Volume</returns> 
    public static Volume Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Объем должен быть больше 0", nameof(value));
        
        return new Volume(value);
    }
    
    /// <summary>
    /// Проверяет, может ли текущий объем вместить указанный объем
    /// </summary>
    /// <param name="other">Объем для проверки</param>
    /// <returns>true если может вместить, false если нет</returns>
    public bool CanAccommodate(Volume other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return Value >= other.Value;
    }
    
    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    /// <summary>
    /// Строковое представление объема
    /// </summary>
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"{Value} ед.";
    }
}