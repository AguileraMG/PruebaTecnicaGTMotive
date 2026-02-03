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
    /// Infrastructure tests for <see cref="IVehicleRepository"/>.
    /// These tests validate real database operations against a MongoDB test instance.
    /// </summary>
    public class VehicleRepositoryTests : InfrastructureTestBase, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly IVehicleRepository _repository;
        private readonly MongoService _mongoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleRepositoryTests"/> class.
        /// </summary>
        /// <param name="fixture">Test server fixture with configured dependencies.</param>
        public VehicleRepositoryTests(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
            _scope = fixture.Server.Services.CreateScope();
            _repository = _scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            _mongoService = _scope.ServiceProvider.GetRequiredService<MongoService>();
        }

        /// <summary>
        /// Initializes the test by cleaning the database before each test runs.
        /// </summary>
        public async Task InitializeAsync()
        {
            var database = _mongoService.MongoClient.GetDatabase("RentalTestDb");
            await database.DropCollectionAsync("Vehicles");
        }

        /// <summary>
        /// Cleanup after test execution.
        /// </summary>
        public Task DisposeAsync()
        {
            _scope?.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Verifies that a vehicle can be successfully persisted to the database
        /// and retrieved with all its properties intact.
        /// </summary>
        [Fact]
        public async Task AddAsync_ShouldPersistVehicleToDatabase()
        {
            // Arrange - Use factory method
            var vehicle = Vehicle.Create(
                brand: "Toyota",
                model: "Corolla",
                year: 2023,
                licensePlate: "ABC-1234",
                kilometersDriven: 5000);

            // Act
            await _repository.AddAsync(vehicle, CancellationToken.None);
            var result = await _repository.GetByIdAsync(vehicle.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(vehicle.Id);
            result.LicensePlate.Should().Be(vehicle.LicensePlate);
            result.Brand.Should().Be(vehicle.Brand);
            result.Model.Should().Be(vehicle.Model);
            result.Year.Should().Be(vehicle.Year);
            result.Status.Should().Be(VehicleStatus.Available);
        }

        /// <summary>
        /// Verifies that a vehicle can be found by its license plate.
        /// </summary>
        [Fact]
        public async Task GetByLicensePlateAsync_ShouldReturnVehicle_WhenExists()
        {
            // Arrange - Use factory method
            var vehicle = Vehicle.Create(
                brand: "Honda",
                model: "Civic",
                year: 2022,
                licensePlate: "XYZ-9876",
                kilometersDriven: 10000);

            await _repository.AddAsync(vehicle, CancellationToken.None);

            // Act
            var result = await _repository.GetByLicensePlateAsync(vehicle.LicensePlate, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(vehicle.Id);
            result.LicensePlate.Should().Be(vehicle.LicensePlate);
        }

        /// <summary>
        /// Verifies that GetByLicensePlateAsync returns null when the vehicle doesn't exist.
        /// </summary>
        [Fact]
        public async Task GetByLicensePlateAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByLicensePlateAsync("NON-EXISTENT", CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Verifies that a vehicle's status can be updated in the database.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ShouldModifyVehicleStatusInDatabase()
        {
            // Arrange - Use factory method
            var vehicle = Vehicle.Create(
                brand: "Ford",
                model: "Focus",
                year: 2021,
                licensePlate: "DEF-5555",
                kilometersDriven: 15000);

            await _repository.AddAsync(vehicle, CancellationToken.None);

            // Act - Use domain method
            vehicle.MarkAsRented();
            await _repository.UpdateAsync(vehicle, CancellationToken.None);
            var result = await _repository.GetByIdAsync(vehicle.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(VehicleStatus.Rented);
        }

        /// <summary>
        /// Verifies that vehicles can be filtered by their status.
        /// </summary>
        [Fact]
        public async Task GetByStatusAsync_ShouldReturnOnlyVehiclesWithSpecifiedStatus()
        {
            // Arrange - Use factory methods
            var availableVehicle = Vehicle.Create(
                brand: "Tesla",
                model: "Model 3",
                year: 2024,
                licensePlate: "TSL-2024",
                kilometersDriven: 2000);

            var rentedVehicle = Vehicle.Create(
                brand: "BMW",
                model: "X5",
                year: 2023,
                licensePlate: "BMW-7890",
                kilometersDriven: 8000);

            // Mark second vehicle as rented
            rentedVehicle.MarkAsRented();

            await _repository.AddAsync(availableVehicle, CancellationToken.None);
            await _repository.AddAsync(rentedVehicle, CancellationToken.None);

            // Act
            var availableVehicles = await _repository.GetVehiclesByStatusAsync(VehicleStatus.Available, CancellationToken.None);

            // Assert
            availableVehicles.Should().NotBeNull();
            availableVehicles.Should().HaveCount(1);
            availableVehicles.Should().Contain(v => v.LicensePlate == "TSL-2024");
            availableVehicles.Should().NotContain(v => v.LicensePlate == "BMW-7890");
        }

        /// <summary>
        /// Verifies that GetVehiclesByStatusAsync returns all vehicles when status is null.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllVehicles()
        {
            // Arrange - Use factory methods
            var vehicle1 = Vehicle.Create(
                brand: "Audi",
                model: "A4",
                year: 2023,
                licensePlate: "AUD-1111",
                kilometersDriven: 12000);

            var vehicle2 = Vehicle.Create(
                brand: "Mercedes",
                model: "C-Class",
                year: 2022,
                licensePlate: "MER-2222",
                kilometersDriven: 50000);

            // Mark second vehicle as retired
            vehicle2.MarkAsRetired();

            await _repository.AddAsync(vehicle1, CancellationToken.None);
            await _repository.AddAsync(vehicle2, CancellationToken.None);

            // Act
            var allVehicles = await _repository.GetVehiclesByStatusAsync(null, CancellationToken.None);

            // Assert
            allVehicles.Should().NotBeNull();
            allVehicles.Should().HaveCountGreaterOrEqualTo(2);
            allVehicles.Should().Contain(v => v.LicensePlate == "AUD-1111");
            allVehicles.Should().Contain(v => v.LicensePlate == "MER-2222");
        }
    }
}
