using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Rentals
{
    /// <summary>
    /// Functional tests for Rental operations.
    /// These tests validate end-to-end scenarios through the complete application stack.
    /// </summary>
    public class RentalFunctionalTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        /// <summary>
        /// Verifies the complete rental flow: create vehicle, create customer, rent vehicle.
        /// </summary>
        [Fact]
        public async Task CompleteRentalFlowShouldSucceed()
        {
            // Arrange - Create Vehicle
            var currentYear = DateTime.UtcNow.Year;
            var vehicleInput = new CreateVehicleInput
            {
                Brand = "BMW",
                Model = "X5",
                Year = currentYear - 1,
                LicensePlate = $"RENT-{Guid.NewGuid().ToString()[..4]}",
                KilometersDriven = 20000
            };

            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

            var createVehicleOutputPort = new TestCreateVehicleOutputPort();
            var createVehicleUseCase = new CreateVehicleUseCase(createVehicleOutputPort, vehicleRepository);
            await createVehicleUseCase.ExecuteAsync(vehicleInput, CancellationToken.None);
            createVehicleOutputPort.WasStandardHandled.Should().BeTrue();
            var vehicleId = createVehicleOutputPort.Output.Id;

            // Arrange - Create Customer
            var customerInput = new CreateCustomerInput
            {
                Name = "Jane Smith",
                Email = $"jane.{Guid.NewGuid().ToString()[..8]}@example.com",
                PhoneNumber = "+34600555666",
                DriverLicenseNumber = $"DL{Guid.NewGuid().ToString()[..8]}"
            };

            var createCustomerOutputPort = new TestCreateCustomerOutputPort();
            var createCustomerUseCase = new CreateCustomerUseCase(createCustomerOutputPort, customerRepository);
            await createCustomerUseCase.ExecuteAsync(customerInput, CancellationToken.None);
            createCustomerOutputPort.WasStandardHandled.Should().BeTrue();
            var customerId = createCustomerOutputPort.Output.Id;

            // Act - Rent Vehicle
            var rentInput = new RentVehicleInput
            {
                VehicleId = vehicleId,
                CustomerId = customerId,
                ExpectedReturnDate = DateTime.UtcNow.AddDays(7)
            };

            var rentOutputPort = new TestRentVehicleOutputPort();
            var rentUseCase = new RentVehicleUseCase(rentOutputPort, vehicleRepository, customerRepository, rentalRepository);
            await rentUseCase.ExecuteAsync(rentInput, CancellationToken.None);

            // Assert
            rentOutputPort.WasStandardHandled.Should().BeTrue();
            rentOutputPort.Output.Should().NotBeNull();
            rentOutputPort.Output.RentalId.Should().NotBeNullOrEmpty();
            rentOutputPort.Output.VehicleId.Should().Be(vehicleId);
            rentOutputPort.Output.CustomerId.Should().Be(customerId);

            // Verify vehicle is rented
            var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, CancellationToken.None);
            vehicle.Status.Should().Be(VehicleStatus.Rented);

            // Verify customer has active rental
            var customer = await customerRepository.GetByIdAsync(customerId, CancellationToken.None);
            customer.HasActiveRental.Should().BeTrue();
        }

        /// <summary>
        /// Verifies the complete return vehicle flow.
        /// </summary>
        [Fact]
        public async Task CompleteReturnVehicleFlowShouldSucceed()
        {
            // Arrange - Setup: Create vehicle, customer, and rental
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var (vehicleId, customerId, rentalId, _) = await SetupRentalAsync(scope);

            var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

            // Act - Return Vehicle
            var returnInput = new ReturnVehicleInput
            {
                RentalId = rentalId,
                CurrentKilometers = 25000
            };

            var returnOutputPort = new TestReturnVehicleOutputPort();
            var returnUseCase = new ReturnVehicleUseCase(returnOutputPort, vehicleRepository, customerRepository, rentalRepository);
            await returnUseCase.ExecuteAsync(returnInput, CancellationToken.None);

            // Assert
            returnOutputPort.WasStandardHandled.Should().BeTrue();
            returnOutputPort.Output.Should().NotBeNull();
            returnOutputPort.Output.RentalId.Should().Be(rentalId);

            // Verify rental is completed
            var rental = await rentalRepository.GetByIdAsync(rentalId, CancellationToken.None);
            rental.Status.Should().Be(RentalStatus.Completed);
            rental.ReturnDate.Should().NotBeNull();

            // Verify vehicle kilometers updated
            var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, CancellationToken.None);
            vehicle.KilometersDriven.Should().Be(25000);

            // Verify customer has no active rental
            var customer = await customerRepository.GetByIdAsync(customerId, CancellationToken.None);
            customer.HasActiveRental.Should().BeFalse();
        }

        /// <summary>
        /// Verifies that getting a rental by license plate works correctly.
        /// </summary>
        [Fact]
        public async Task GetRentalByLicensePlateShouldReturnActiveRental()
        {
            // Arrange - Setup: Create vehicle, customer, and rental
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var (vehicleId, _, rentalId, licensePlate) = await SetupRentalAsync(scope);

            var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

            // Act - Get rental by license plate
            var input = new GetRentalByLicensePlateInput
            {
                LicensePlate = licensePlate
            };

            var outputPort = new TestGetRentalByLicensePlateOutputPort();
            var useCase = new GetRentalByLicensePlateUseCase(outputPort, vehicleRepository, rentalRepository);
            await useCase.ExecuteAsync(input, CancellationToken.None);

            // Assert
            outputPort.WasStandardHandled.Should().BeTrue();
            outputPort.Output.Should().NotBeNull();
            outputPort.Output.RentalId.Should().Be(rentalId);
            outputPort.Output.VehicleId.Should().Be(vehicleId);
            outputPort.Output.VehicleLicensePlate.Should().Be(licensePlate);
        }

        /// <summary>
        /// Verifies that getting all rentals returns the correct count.
        /// </summary>
        [Fact]
        public async Task GetAllRentalsShouldReturnAllActiveRentals()
        {
            // Arrange - Create 3 rentals
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);

            for (var i = 0; i < 3; i++)
            {
                await SetupRentalAsync(scope);
            }

            var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

            // Act
            var getAllInput = new GetAllRentalsInput();
            var getAllOutputPort = new TestGetAllRentalsOutputPort();
            var getAllUseCase = new GetAllRentalsUseCase(getAllOutputPort, rentalRepository);
            await getAllUseCase.ExecuteAsync(getAllInput, CancellationToken.None);

            // Assert
            getAllOutputPort.WasStandardHandled.Should().BeTrue();
            getAllOutputPort.Output.Should().NotBeNull();
            getAllOutputPort.Output.Rentals.Should().HaveCount(3);
            getAllOutputPort.Output.TotalCount.Should().Be(3);
        }

        /// <summary>
        /// Verifies that a customer cannot rent multiple vehicles simultaneously.
        /// </summary>
        [Fact]
        public async Task CustomerWithActiveRentalCannotRentAgain()
        {
            // Arrange - Setup: Create first rental
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var (_, customerId, _, _) = await SetupRentalAsync(scope);

            var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

            // Create second vehicle
            var currentYear = DateTime.UtcNow.Year;
            var vehicle2Input = new CreateVehicleInput
            {
                Brand = "Audi",
                Model = "A4",
                Year = currentYear - 1,
                LicensePlate = $"SEC-{Guid.NewGuid().ToString()[..4]}",
                KilometersDriven = 15000
            };

            var createVehicleOutputPort = new TestCreateVehicleOutputPort();
            var createVehicleUseCase = new CreateVehicleUseCase(createVehicleOutputPort, vehicleRepository);
            await createVehicleUseCase.ExecuteAsync(vehicle2Input, CancellationToken.None);
            var vehicleId2 = createVehicleOutputPort.Output.Id;

            // Act - Try to rent second vehicle with same customer
            var rentInput = new RentVehicleInput
            {
                VehicleId = vehicleId2,
                CustomerId = customerId,
                ExpectedReturnDate = DateTime.UtcNow.AddDays(7)
            };

            var rentOutputPort = new TestRentVehicleOutputPort();
            var rentUseCase = new RentVehicleUseCase(rentOutputPort, vehicleRepository, customerRepository, rentalRepository);
            await rentUseCase.ExecuteAsync(rentInput, CancellationToken.None);

            // Assert - Should be ConflictHandled (409) not NotFoundHandled
            rentOutputPort.WasConflictHandled.Should().BeTrue();
            rentOutputPort.WasStandardHandled.Should().BeFalse();
            rentOutputPort.ErrorMessage.Should().Contain("already has an active rental");
        }

        // Helper method to setup a complete rental
        private async Task<(string VehicleId, string CustomerId, string RentalId, string LicensePlate)> SetupRentalAsync(IServiceScope scope)
        {
            var currentYear = DateTime.UtcNow.Year;
            var uniqueId = Guid.NewGuid().ToString()[..8];

            var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var rentalRepository = scope.ServiceProvider.GetRequiredService<IRentalRepository>();

            // Create Vehicle
            var vehicleInput = new CreateVehicleInput
            {
                Brand = "Mercedes",
                Model = "C-Class",
                Year = currentYear - 1,
                LicensePlate = $"TEST-{uniqueId}",
                KilometersDriven = 20000
            };

            var createVehicleOutputPort = new TestCreateVehicleOutputPort();
            var createVehicleUseCase = new CreateVehicleUseCase(createVehicleOutputPort, vehicleRepository);
            await createVehicleUseCase.ExecuteAsync(vehicleInput, CancellationToken.None);
            var vehicleId = createVehicleOutputPort.Output.Id;
            var licensePlate = createVehicleOutputPort.Output.LicensePlate;

            // Create Customer
            var customerInput = new CreateCustomerInput
            {
                Name = $"Test Customer {uniqueId}",
                Email = $"test.{uniqueId}@example.com",
                PhoneNumber = $"+346001234{uniqueId[..2]}",
                DriverLicenseNumber = $"DL{uniqueId}"
            };

            var createCustomerOutputPort = new TestCreateCustomerOutputPort();
            var createCustomerUseCase = new CreateCustomerUseCase(createCustomerOutputPort, customerRepository);
            await createCustomerUseCase.ExecuteAsync(customerInput, CancellationToken.None);
            var customerId = createCustomerOutputPort.Output.Id;

            // Create Rental
            var rentInput = new RentVehicleInput
            {
                VehicleId = vehicleId,
                CustomerId = customerId,
                ExpectedReturnDate = DateTime.UtcNow.AddDays(7)
            };

            var rentOutputPort = new TestRentVehicleOutputPort();
            var rentUseCase = new RentVehicleUseCase(rentOutputPort, vehicleRepository, customerRepository, rentalRepository);
            await rentUseCase.ExecuteAsync(rentInput, CancellationToken.None);
            var rentalId = rentOutputPort.Output.RentalId;

            return (vehicleId, customerId, rentalId, licensePlate);
        }
    }

    // Test Output Ports for Rentals
    internal class TestRentVehicleOutputPort : IRentVehicleOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public bool WasBadRequestHandled { get; private set; }

        public bool WasNotFoundHandled { get; private set; }

        public bool WasConflictHandled { get; private set; }

        public RentVehicleOutput Output { get; private set; }

        public string ErrorMessage { get; private set; }

        public void StandardHandle(RentVehicleOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }

        public void BadRequestHandle(string message)
        {
            WasBadRequestHandled = true;
            ErrorMessage = message;
        }

        public void NotFoundHandle(string message)
        {
            WasNotFoundHandled = true;
            ErrorMessage = message;
        }

        public void ConflictHandle(string message)
        {
            WasConflictHandled = true;
            ErrorMessage = message;
        }
    }

    internal class TestReturnVehicleOutputPort : IReturnVehicleOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public bool WasBadRequestHandled { get; private set; }

        public bool WasNotFoundHandled { get; private set; }

        public bool WasConflictHandled { get; private set; }

        public ReturnVehicleOutput Output { get; private set; }

        public string ErrorMessage { get; private set; }

        public void StandardHandle(ReturnVehicleOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }

        public void BadRequestHandle(string message)
        {
            WasBadRequestHandled = true;
            ErrorMessage = message;
        }

        public void NotFoundHandle(string message)
        {
            WasNotFoundHandled = true;
            ErrorMessage = message;
        }

        public void ConflictHandle(string message)
        {
            WasConflictHandled = true;
            ErrorMessage = message;
        }
    }

    internal class TestGetRentalByLicensePlateOutputPort : IGetRentalByLicensePlateOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public bool WasBadRequestHandled { get; private set; }

        public bool WasNotFoundHandled { get; private set; }

        public GetRentalByLicensePlateOutput Output { get; private set; }

        public string ErrorMessage { get; private set; }

        public void StandardHandle(GetRentalByLicensePlateOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }

        public void BadRequestHandle(string message)
        {
            WasBadRequestHandled = true;
            ErrorMessage = message;
        }

        public void NotFoundHandle(string message)
        {
            WasNotFoundHandled = true;
            ErrorMessage = message;
        }
    }

    internal class TestGetAllRentalsOutputPort : IGetAllRentalsOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public GetAllRentalsOutput Output { get; private set; }

        public void StandardHandle(GetAllRentalsOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }
    }

    internal class TestCreateVehicleOutputPort : ICreateVehicleOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public bool WasBadRequestHandled { get; private set; }

        public bool WasConflictHandled { get; private set; }

        public CreateVehicleOutput Output { get; private set; }

        public string ErrorMessage { get; private set; }

        public void StandardHandle(CreateVehicleOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }

        public void BadRequestHandle(string message)
        {
            WasBadRequestHandled = true;
            ErrorMessage = message;
        }

        public void ConflictHandle(string message)
        {
            WasConflictHandled = true;
            ErrorMessage = message;
        }
    }

    internal class TestCreateCustomerOutputPort : ICreateCustomerOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public bool WasConflictHandled { get; private set; }

        public CreateCustomerOutput Output { get; private set; }

        public string ErrorMessage { get; private set; }

        public void StandardHandle(CreateCustomerOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }

        public void ConflictHandle(string message)
        {
            WasConflictHandled = true;
            ErrorMessage = message;
        }
    }
}
