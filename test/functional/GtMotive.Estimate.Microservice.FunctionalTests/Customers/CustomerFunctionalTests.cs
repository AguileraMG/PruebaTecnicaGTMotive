using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Customers
{
    /// <summary>
    /// Functional tests for Customer operations.
    /// These tests validate end-to-end scenarios through the complete application stack.
    /// </summary>
    public class CustomerFunctionalTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        /// <summary>
        /// Verifies that a customer can be created successfully through the complete flow.
        /// </summary>
        [Fact]
        public async Task CreateCustomerCompleteFlowShouldSucceed()
        {
            // Arrange
            var input = new CreateCustomerInput
            {
                Name = "John Doe",
                Email = $"john.doe.{Guid.NewGuid().ToString()[..8]}@example.com",
                PhoneNumber = "+34600123456",
                DriverLicenseNumber = $"DL{Guid.NewGuid().ToString()[..8]}"
            };

            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var outputPort = new TestCreateCustomerOutputPort();
            var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var useCase = new CreateCustomerUseCase(outputPort, repository);

            // Act
            await useCase.ExecuteAsync(input, CancellationToken.None);

            // Assert
            outputPort.WasStandardHandled.Should().BeTrue();
            outputPort.Output.Should().NotBeNull();
            outputPort.Output.Id.Should().NotBeNullOrEmpty();
            outputPort.Output.Name.Should().Be(input.Name);
            outputPort.Output.Email.Should().Be(input.Email);
            outputPort.Output.PhoneNumber.Should().Be(input.PhoneNumber);

            // Verify in database
            var savedCustomer = await repository.GetByIdAsync(outputPort.Output.Id, CancellationToken.None);
            savedCustomer.Should().NotBeNull();
            savedCustomer.Name.Should().Be(input.Name);
            savedCustomer.HasActiveRental.Should().BeFalse();
        }

        /// <summary>
        /// Verifies that creating a customer with duplicate email is rejected.
        /// </summary>
        [Fact]
        public async Task CreateCustomerWithDuplicateEmailShouldFail()
        {
            // Arrange - Create first customer
            var email = $"duplicate.{Guid.NewGuid().ToString()[..8]}@example.com";
            var input1 = new CreateCustomerInput
            {
                Name = "First Customer",
                Email = email,
                PhoneNumber = "+34600111111",
                DriverLicenseNumber = $"DL{Guid.NewGuid().ToString()[..8]}"
            };

            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var outputPort1 = new TestCreateCustomerOutputPort();
            var useCase1 = new CreateCustomerUseCase(outputPort1, repository);

            await useCase1.ExecuteAsync(input1, CancellationToken.None);
            outputPort1.WasStandardHandled.Should().BeTrue();

            // Act - Try to create second customer with same email
            var input2 = new CreateCustomerInput
            {
                Name = "Second Customer",
                Email = email,
                PhoneNumber = "+34600222222",
                DriverLicenseNumber = $"DL{Guid.NewGuid().ToString()[..8]}"
            };
            var outputPort2 = new TestCreateCustomerOutputPort();
            var useCase2 = new CreateCustomerUseCase(outputPort2, repository);

            await useCase2.ExecuteAsync(input2, CancellationToken.None);

            // Assert - Should be ConflictHandled (409) not NotFoundHandled
            outputPort2.WasConflictHandled.Should().BeTrue();
            outputPort2.WasStandardHandled.Should().BeFalse();
            outputPort2.ErrorMessage.Should().Contain("already exists");
        }

        /// <summary>
        /// Verifies the complete flow of creating multiple customers and retrieving all.
        /// </summary>
        [Fact]
        public async Task CreateMultipleCustomersAndGetAllShouldWork()
        {
            // Arrange - Create 5 customers
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

            for (var i = 0; i < 5; i++)
            {
                var input = new CreateCustomerInput
                {
                    Name = $"Customer {i}",
                    Email = $"customer{i}.{Guid.NewGuid().ToString()[..8]}@example.com",
                    PhoneNumber = $"+3460012345{i}",
                    DriverLicenseNumber = $"DL{Guid.NewGuid().ToString()[..8]}"
                };

                var createOutputPort = new TestCreateCustomerOutputPort();
                var createUseCase = new CreateCustomerUseCase(createOutputPort, repository);
                await createUseCase.ExecuteAsync(input, CancellationToken.None);
                createOutputPort.WasStandardHandled.Should().BeTrue();
            }

            // Act - Get all customers
            var getAllInput = new GetAllCustomersInput();
            var getAllOutputPort = new TestGetAllCustomersOutputPort();
            var getAllUseCase = new GetAllCustomersUseCase(getAllOutputPort, repository);

            await getAllUseCase.ExecuteAsync(getAllInput, CancellationToken.None);

            // Assert
            getAllOutputPort.WasStandardHandled.Should().BeTrue();
            getAllOutputPort.Output.Should().NotBeNull();
            getAllOutputPort.Output.Customers.Should().HaveCount(5);
            getAllOutputPort.Output.TotalCount.Should().Be(5);
        }

        /// <summary>
        /// Verifies that getting all customers when none exist returns empty list.
        /// </summary>
        [Fact]
        public async Task GetAllCustomersWhenNoneExistShouldReturnEmpty()
        {
            // Arrange
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var input = new GetAllCustomersInput();
            var outputPort = new TestGetAllCustomersOutputPort();
            var useCase = new GetAllCustomersUseCase(outputPort, repository);

            // Act
            await useCase.ExecuteAsync(input, CancellationToken.None);

            // Assert
            outputPort.WasStandardHandled.Should().BeTrue();
            outputPort.Output.Should().NotBeNull();
            outputPort.Output.Customers.Should().BeEmpty();
            outputPort.Output.TotalCount.Should().Be(0);
        }
    }
}
