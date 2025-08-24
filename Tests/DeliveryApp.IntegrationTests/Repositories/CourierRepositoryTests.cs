using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryShould : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17.5")
        .WithDatabase("order")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private ApplicationDbContext _context;
    
    public CourierRepositoryShould()
    {
    }
    
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); })
            .Options;
        _context = new ApplicationDbContext(contextOptions);
        _context.Database.Migrate();
    }
    
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CanAddCourier()
    {
        //Arrange
        var courierId = Guid.CreateVersion7();
        var courier = Courier.Create("Courier 1", Speed.Create(3), Location.MinCoordinates);
        courier.AddStoragePlace("Bag", Volume.Create(5));
        courier.AddStoragePlace("Bike", Volume.Create(25));
            
        //Act
        var repository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await repository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getResult = await repository.GetAsync(courier.Id);
        getResult.HasValue.Should().BeTrue();
        var courierFromDb = getResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanAddCourierWithoutStoragePlace()
    {
        //Arrange
        var courier = Courier.Create("Courier 1", Speed.Create(3), Location.MinCoordinates);
        
        //Act
        var repository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await repository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getResult = await repository.GetAsync(courier.Id);
        getResult.HasValue.Should().BeTrue();
        var courierFromDb = getResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async Task CanUpdateCourier()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinCoordinates,Volume.Create(5));
        var courier = Courier.Create( "Courier 1", Speed.Create(1), Location.MinCoordinates);
        
        var repository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await repository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Act
        courier.TakeOrder(order);
        repository.Update(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getResult = await repository.GetAsync(courier.Id);
        getResult.HasValue.Should().BeTrue();
        var courierFromDb = getResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var courier = Courier.Create("Courier 1", Speed.Create(1), Location.MinCoordinates);

        //Act
        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getResult = await courierRepository.GetAsync(courier.Id);
        getResult.HasValue.Should().BeTrue();
        var courierFromDb = getResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async Task CanFindAllFree()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.MinCoordinates,Volume.Create(5));
        
        var courier1 = Courier.Create("Courier1", Speed.Create(1), Location.MinCoordinates);
        courier1.TakeOrder(order);

        var courier2 = Courier.Create( "Courier 2",Speed.Create(1), Location.MinCoordinates);

        var repository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await repository.AddAsync(courier1);
        await repository.AddAsync(courier2);
        await unitOfWork.SaveChangesAsync();

        //Act
        var activeCouriersFromDb = repository.FindAllFree();

        //Assert
        var couriersFromDb = await activeCouriersFromDb;
        couriersFromDb.Should().NotBeEmpty();
        couriersFromDb.Count().Should().Be(1);
        couriersFromDb.First().Should().BeEquivalentTo(courier2);
    }
}