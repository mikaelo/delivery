using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

/// <summary>
/// Координата
/// </summary>
public class Location : ValueObject
{
    public static readonly Location MinCoordinates = new(1, 1);
    public static readonly Location MaxCoordinates = new(10, 10);
    
    /// <summary>
    /// X (горизонталь)
    /// </summary>
    public int X { get; }
    
    /// <summary>
    /// Y (вертикаль)
    /// </summary>
    public int Y { get; }

    [ExcludeFromCodeCoverage]
    private Location()
    {
    }
    
    private Location(int x, int y) :this()
    {
        X = x;
        Y = y;
    }
    
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"(X:{X}, Y:{Y})";
    }
    
    [ExcludeFromCodeCoverage]
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
    
    /// <summary>
    /// Вычисляет манхэттенское расстояние до другой координаты
    /// Расстояние равно сумме модулей разностей координат по X и Y
    /// </summary>
    /// <param name="other">Целевая координата</param>
    /// <returns>Количество шагов для достижения целевой координаты</returns>
    public int DistanceTo(Location other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }
    
    /// <summary>
    /// Создает новую координату, сдвинутую на указанное количество шагов (относительное перемещение)
    /// </summary>
    /// <param name="deltaX">Сдвиг по X</param>
    /// <param name="deltaY">Сдвиг по Y</param>
    /// <returns>Result с новой координатой Location или сообщением об ошибке</returns>
    public Result<Location> Move(int deltaX, int deltaY)
    {
        return Create(X + deltaX, Y + deltaY);
    }
    
    /// <summary>
    /// Создает новую координату в указанной позиции (абсолютное перемещение)
    /// </summary>
    /// <param name="newX">Новая координата X</param>
    /// <param name="newY">Новая координата Y</param>
    /// <returns>Result с новой координатой Location или сообщением об ошибке</returns>
    public Result<Location> MoveTo(int newX, int newY)
    {
        return Create(newX, newY);
    }
    
    /// <summary>
    /// Создает новую координату
    /// </summary>
    /// <param name="x">Координата по горизонтали</param>
    /// <param name="y">Координата по вертикали</param>
    /// <returns>Result с Location или сообщением об ошибке</returns> 
    public static Result<Location> Create(int x, int y)
    {
        if (x < MinCoordinates.X || x > MaxCoordinates.X)
            return Result.Failure<Location>($"Координата X должна быть в диапазоне от {MinCoordinates.X} до {MaxCoordinates.X}. Получено: {x}");
            
        if (y < MinCoordinates.Y || y > MaxCoordinates.Y)
            return Result.Failure<Location>($"Координата Y должна быть в диапазоне от {MinCoordinates.Y} до {MaxCoordinates.Y}. Получено: {y}");
            
        return Result.Success(new Location(x, y));
    }
    
    
    /// <summary>
    /// Создает случайную координату с использованием переданного генератора случайных чисел
    /// </summary>
    /// <param name="random">Генератор случайных чисел</param>
    /// <returns>Новая случайная координата Location</returns> 
    public static Location CreateRandom(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        var randomX = random.Next(MinCoordinates.X, MaxCoordinates.X + 1);
        var randomY = random.Next(MinCoordinates.Y, MaxCoordinates.Y + 1);
        
        return new Location(randomX, randomY);
    }
    
}