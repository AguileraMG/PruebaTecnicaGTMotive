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
    /// Infrastructure tests for <see cref="ICustomerRepository"/>.
    /// These tests validate real database operations against a MongoDB test instance.
    /// </summary>
    public class CustomerRepositoryTests : InfrastructureTestBase, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly ICustomerRepository _repository;
        private readonly MongoService _mongoService;

        public CustomerRepositoryTests(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
            _scope = fixture.Server.Services.CreateScope();
            _repository = _scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            _mongoService = _scope.ServiceProvider.GetRequiredService<MongoService>();
        }

        public async Task InitializeAsync()
        {
            var database = _mongoService.MongoClient.GetDatabase("RentalTestDb");
            await database.DropCollectionAsync("Customers");
        }

        public Task DisposeAsync()
        {
            _scope?.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Verifies that a customer can be successfully persisted to the database
        /// and retrieved with all its properties intact.
        /// </summary>
        [Fact]
        public async Task AddAsync_ShouldPersistCustomerToDatabase()
        {
            // Arrange - Use factory method
            var customer = Customer.Create(
                name: "John Doe",
                email: "john.doe@example.com",
                phoneNumber: "+34600000000",
                driverLicenseNumber: "DL123456");

            // Act
            await _repository.AddAsync(customer, CancellationToken.None);
            var result = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(customer.Id);
            result.Name.Should().Be(customer.Name);
            result.Email.Should().Be(customer.Email);
            result.PhoneNumber.Should().Be(customer.PhoneNumber);
            result.HasActiveRental.Should().BeFalse();
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns null when the customer doesn't exist.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync("non-existent-id", CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Verifies that a customer's HasActiveRental property can be updated.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ShouldModifyCustomerInDatabase()
        {
            // Arrange - Use factory method
            var customer = Customer.Create(
                name: "Jane Smith",
                email: "jane.smith@example.com",
                phoneNumber: "+34611111111",
                driverLicenseNumber: "DL789012");

            await _repository.AddAsync(customer, CancellationToken.None);

            // Act - Use domain method
            customer.MarkAsRenting();
            await _repository.UpdateAsync(customer, CancellationToken.None);
            var result = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.HasActiveRental.Should().BeTrue();
        }

        /// <summary>
        /// Verifies that GetAllAsync returns all customers in the database.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            // Arrange - Use factory methods
            var customer1 = Customer.Create(
                name: "Alice Johnson",
                email: "alice@example.com",
                phoneNumber: "+34622222222",
                driverLicenseNumber: "DL111222");

            var customer2 = Customer.Create(
                name: "Bob Williams",
                email: "bob@example.com",
                phoneNumber: "+34633333333",
                driverLicenseNumber: "DL333444");

            // Mark second customer as renting
            customer2.MarkAsRenting();

            await _repository.AddAsync(customer1, CancellationToken.None);
            await _repository.AddAsync(customer2, CancellationToken.None);

            // Act
            var allCustomers = await _repository.GetAllAsync(null, CancellationToken.None);

            // Assert
            allCustomers.Should().NotBeNull();
            allCustomers.Should().HaveCountGreaterOrEqualTo(2);
            allCustomers.Should().Contain(c => c.Email == "alice@example.com");
            allCustomers.Should().Contain(c => c.Email == "bob@example.com");
        }

        /// <summary>
        /// Verifies that multiple operations on the same customer work correctly.
        /// </summary>
        [Fact]
        public async Task MultipleOperations_ShouldWorkCorrectly()
        {
            // Arrange - Use factory method
            var customer = Customer.Create(
                name: "Test Customer",
                email: "test@example.com",
                phoneNumber: "+34644444444",
                driverLicenseNumber: "DL555666");

            // Act & Assert - Add
            await _repository.AddAsync(customer, CancellationToken.None);
            var afterAdd = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);
            afterAdd.Should().NotBeNull();
            afterAdd.HasActiveRental.Should().BeFalse();

            // Act & Assert - Update to active rental using domain method
            customer.MarkAsRenting();
            await _repository.UpdateAsync(customer, CancellationToken.None);
            var afterUpdate1 = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);
            afterUpdate1.HasActiveRental.Should().BeTrue();

            // Act & Assert - Update back to no active rental using domain method
            customer.MarkAsNotRenting();
            await _repository.UpdateAsync(customer, CancellationToken.None);
            var afterUpdate2 = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);
            afterUpdate2.HasActiveRental.Should().BeFalse();
        }
    }
}
