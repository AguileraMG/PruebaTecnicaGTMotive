using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Repositories
{
    /// <summary>
    /// Infrastructure tests for <see cref="IRentalRepository"/>.
    /// These tests validate real database operations against a MongoDB test instance.
    /// </summary>
    public class RentalRepositoryTests : InfrastructureTestBase, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly IRentalRepository _repository;
        private readonly MongoService _mongoService;

        public RentalRepositoryTests(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
            _scope = fixture.Server.Services.CreateScope();
            _repository = _scope.ServiceProvider.GetRequiredService<IRentalRepository>();
            _mongoService = _scope.ServiceProvider.GetRequiredService<MongoService>();
        }

        public async Task InitializeAsync()
        {
            var database = _mongoService.MongoClient.GetDatabase("RentalTestDb");
            await database.DropCollectionAsync("Rentals");
        }

        public Task DisposeAsync()
        {
            _scope?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddAsyncShouldPersistRentalToDatabase()
        {
            // Arrange - Use factory method
            var vehicleId = Guid.NewGuid().ToString();
            var customerId = Guid.NewGuid().ToString();
            var rental = Rental.Create(
                vehicleId: vehicleId,
                customerId: customerId,
                expectedReturnDate: DateTime.UtcNow.AddDays(5),
                notes: "Test rental");

            // Act
            await _repository.AddAsync(rental, CancellationToken.None);
            var result = await _repository.GetByIdAsync(rental.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(rental.Id);
            result.VehicleId.Should().Be(rental.VehicleId);
            result.CustomerId.Should().Be(rental.CustomerId);
            result.Status.Should().Be(RentalStatus.Active);
            result.IsActive().Should().BeTrue();
            result.ReturnDate.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnNullWhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync("non-existent-id", CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsyncShouldMarkRentalAsCompleted()
        {
            // Arrange - Use factory method
            var rental = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(2));

            await _repository.AddAsync(rental, CancellationToken.None);

            // Act - Use domain method
            rental.CompleteRental();
            await _repository.UpdateAsync(rental, CancellationToken.None);
            var result = await _repository.GetByIdAsync(rental.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsActive().Should().BeFalse();
            result.Status.Should().Be(RentalStatus.Completed);
            result.ReturnDate.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllRentalsAsyncShouldReturnAllRentals()
        {
            // Arrange - Use factory methods
            var rental1 = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(7));

            var rental2 = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(3));

            // Complete second rental
            rental2.CompleteRental();

            await _repository.AddAsync(rental1, CancellationToken.None);
            await _repository.AddAsync(rental2, CancellationToken.None);

            // Act
            var allRentals = await _repository.GetAllRentalsAsync(null, CancellationToken.None);

            // Assert
            allRentals.Should().NotBeNull();
            allRentals.Should().HaveCountGreaterOrEqualTo(2);
            allRentals.Should().Contain(r => r.Id == rental1.Id);
            allRentals.Should().Contain(r => r.Id == rental2.Id);
        }

        [Fact]
        public async Task GetActiveRentalsAsyncShouldReturnOnlyActiveRentals()
        {
            // Arrange - Use factory methods
            var activeRental = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(5));

            var completedRental = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(3));

            // Complete second rental
            completedRental.CompleteRental();

            await _repository.AddAsync(activeRental, CancellationToken.None);
            await _repository.AddAsync(completedRental, CancellationToken.None);

            // Act
            var activeRentals = await _repository.GetActiveRentalsAsync(CancellationToken.None);

            // Assert
            activeRentals.Should().NotBeNull();
            activeRentals.Should().Contain(r => r.Id == activeRental.Id);
            activeRentals.Should().NotContain(r => r.Id == completedRental.Id);
            activeRentals.All(r => r.IsActive()).Should().BeTrue();
        }

        [Fact]
        public async Task GetActiveRentalByVehicleIdAsyncShouldReturnActiveRental()
        {
            // Arrange - Use factory method
            var vehicleId = Guid.NewGuid().ToString();
            var rental = Rental.Create(
                vehicleId: vehicleId,
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(3));

            await _repository.AddAsync(rental, CancellationToken.None);

            // Act
            var result = await _repository.GetActiveRentalByVehicleIdAsync(vehicleId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.VehicleId.Should().Be(vehicleId);
            result.IsActive().Should().BeTrue();
        }

        [Fact]
        public async Task GetActiveRentalsByCustomerIdAsyncShouldReturnCustomerActiveRentals()
        {
            // Arrange - Use factory methods
            var customerId = Guid.NewGuid().ToString();
            var activeRental = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: customerId,
                expectedReturnDate: DateTime.UtcNow.AddDays(5));

            var completedRental = Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: customerId,
                expectedReturnDate: DateTime.UtcNow.AddDays(2));

            // Complete second rental
            completedRental.CompleteRental();

            await _repository.AddAsync(activeRental, CancellationToken.None);
            await _repository.AddAsync(completedRental, CancellationToken.None);

            // Act
            var result = await _repository.GetActiveRentalsByCustomerIdAsync(customerId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain(r => r.Id == activeRental.Id);
            result.Should().NotContain(r => r.Id == completedRental.Id);
        }
    }
}
