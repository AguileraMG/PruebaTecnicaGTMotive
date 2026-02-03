using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Vehicles
{
    /// <summary>
    /// Functional tests for Vehicle operations.
    /// These tests validate end-to-end scenarios through the complete application stack.
    /// </summary>
    public class VehicleFunctionalTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        /// <summary>
        /// Verifies that a vehicle can be created successfully through the complete flow.
        /// </summary>
        [Fact]
        public async Task CreateVehicleCompleteFlowShouldSucceed()
        {
            // Arrange
            var currentYear = DateTime.UtcNow.Year;
            var input = new CreateVehicleInput
            {
                Brand = "Toyota",
                Model = "Camry",
                Year = currentYear - 2,
                LicensePlate = $"TEST-{Guid.NewGuid().ToString()[..4]}",
                KilometersDriven = 50000
            };

            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var outputPort = new TestCreateVehicleOutputPort();
            var repository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var useCase = new CreateVehicleUseCase(outputPort, repository);

            // Act
            await useCase.ExecuteAsync(input, CancellationToken.None);

            // Assert
            outputPort.WasStandardHandled.Should().BeTrue();
            outputPort.Output.Should().NotBeNull();
            outputPort.Output.Id.Should().NotBeNullOrEmpty();
            outputPort.Output.Brand.Should().Be(input.Brand);
            outputPort.Output.Model.Should().Be(input.Model);
            outputPort.Output.LicensePlate.Should().Be(input.LicensePlate);
            outputPort.Output.Status.Should().Be(VehicleStatus.Available.ToString());

            // Verify in database
            var savedVehicle = await repository.GetByIdAsync(outputPort.Output.Id, CancellationToken.None);
            savedVehicle.Should().NotBeNull();
            savedVehicle.Brand.Should().Be(input.Brand);
        }

        /// <summary>
        /// Verifies that creating a vehicle that is too old is rejected.
        /// </summary>
        [Fact]
        public async Task CreateVehicleTooOldShouldFail()
        {
            // Arrange
            var currentYear = DateTime.UtcNow.Year;
            var input = new CreateVehicleInput
            {
                Brand = "Honda",
                Model = "Accord",
                Year = currentYear - 10,
                LicensePlate = $"OLD-{Guid.NewGuid().ToString()[..4]}",
                KilometersDriven = 150000
            };

            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var outputPort = new TestCreateVehicleOutputPort();
            var repository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var useCase = new CreateVehicleUseCase(outputPort, repository);

            // Act
            await useCase.ExecuteAsync(input, CancellationToken.None);

            // Assert - Should be BadRequestHandled (400) not NotFoundHandled
            outputPort.WasBadRequestHandled.Should().BeTrue();
            outputPort.WasStandardHandled.Should().BeFalse();
            outputPort.ErrorMessage.Should().Contain("too old");
        }

        /// <summary>
        /// Verifies the complete flow of creating multiple vehicles and filtering by status.
        /// </summary>
        [Fact]
        public async Task CreateMultipleVehiclesAndFilterByStatusShouldWork()
        {
            // Arrange - Create 3 available vehicles
            var currentYear = DateTime.UtcNow.Year;
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var repository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();

            for (var i = 0; i < 3; i++)
            {
                var input = new CreateVehicleInput
                {
                    Brand = $"Brand{i}",
                    Model = $"Model{i}",
                    Year = currentYear - 1,
                    LicensePlate = $"VEH{i}-{Guid.NewGuid().ToString()[..4]}",
                    KilometersDriven = 10000 * (i + 1)
                };

                var createOutputPort = new TestCreateVehicleOutputPort();
                var createUseCase = new CreateVehicleUseCase(createOutputPort, repository);
                await createUseCase.ExecuteAsync(input, CancellationToken.None);
                createOutputPort.WasStandardHandled.Should().BeTrue();
            }

            // Act - Get all available vehicles
            var getByStatusInput = new GetVehiclesByStatusInput
            {
                Status = VehicleStatus.Available
            };
            var getByStatusOutputPort = new TestGetVehiclesByStatusOutputPort();
            var getByStatusUseCase = new GetVehiclesByStatusUseCase(getByStatusOutputPort, repository);

            await getByStatusUseCase.ExecuteAsync(getByStatusInput, CancellationToken.None);

            // Assert
            getByStatusOutputPort.WasStandardHandled.Should().BeTrue();
            getByStatusOutputPort.Output.Should().NotBeNull();
            getByStatusOutputPort.Output.Vehicles.Should().HaveCount(3);
            getByStatusOutputPort.Output.Vehicles.Should().AllSatisfy(v =>
                v.Status.Should().Be(VehicleStatus.Available.ToString()));
        }

        /// <summary>
        /// Verifies that querying for vehicles with a status that has no matches returns empty list.
        /// </summary>
        [Fact]
        public async Task GetVehiclesByStatusWithNoMatchesShouldReturnEmpty()
        {
            // Arrange
            using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
            var repository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
            var input = new GetVehiclesByStatusInput
            {
                Status = VehicleStatus.Retired
            };
            var outputPort = new TestGetVehiclesByStatusOutputPort();
            var useCase = new GetVehiclesByStatusUseCase(outputPort, repository);

            // Act
            await useCase.ExecuteAsync(input, CancellationToken.None);

            // Assert
            outputPort.WasStandardHandled.Should().BeTrue();
            outputPort.Output.Should().NotBeNull();
            outputPort.Output.Vehicles.Should().BeEmpty();
            outputPort.Output.TotalCount.Should().Be(0);
        }
    }

    // Test Output Ports
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

    internal class TestGetVehiclesByStatusOutputPort : IGetVehiclesByStatusOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public GetVehiclesByStatusOutput Output { get; private set; }

        public void StandardHandle(GetVehiclesByStatusOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }
    }
}
