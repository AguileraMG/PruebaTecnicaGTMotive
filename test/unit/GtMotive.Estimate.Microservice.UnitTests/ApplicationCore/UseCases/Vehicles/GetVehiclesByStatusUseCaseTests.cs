using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Unit tests for the <see cref="GetVehiclesByStatusUseCase"/> class.
    /// Tests validate vehicle retrieval filtered by status with proper data mapping
    /// and handling of empty result sets.
    /// </summary>
    public class GetVehiclesByStatusUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IGetVehiclesByStatusOutputPort> _outputPortMock;
        private readonly GetVehiclesByStatusUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetVehiclesByStatusUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public GetVehiclesByStatusUseCaseTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _outputPortMock = new Mock<IGetVehiclesByStatusOutputPort>();
            _sut = new GetVehiclesByStatusUseCase(_outputPortMock.Object, _vehicleRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that vehicles are correctly filtered and returned when vehicles with the specified status exist.
        /// </summary>
        /// <param name="status">The vehicle status to filter by (Available, Rented, or Retired).</param>
        /// <remarks>
        /// This parameterized test validates filtering for all vehicle statuses:
        /// - Repository returns vehicles matching the specified status
        /// - Output contains the correct total count of filtered vehicles
        /// - Output port StandardHandle is called with the filtered vehicle list.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Theory]
        [InlineData(VehicleStatus.Available)]
        [InlineData(VehicleStatus.Rented)]
        [InlineData(VehicleStatus.Retired)]
        public async Task ExecuteAsyncWhenVehiclesExistWithStatusShouldReturnFilteredVehicles(VehicleStatus status)
        {
            // Arrange
            var vehicles = EntityFakers.VehicleFaker
                .RuleFor(v => v.Status, status)
                .Generate(3);

            var input = new GetVehiclesByStatusInput { Status = status };

            _vehicleRepositoryMock
                .Setup(x => x.GetVehiclesByStatusAsync(status, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicles);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.GetVehiclesByStatusAsync(status, It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.Is<GetVehiclesByStatusOutput>(o => o.TotalCount == 3)), Times.Once);
        }

        /// <summary>
        /// Verifies that an empty list is returned when no vehicles exist with the specified status.
        /// </summary>
        /// <remarks>
        /// This test validates proper handling of empty result sets:
        /// - Repository returns an empty list for the Available status
        /// - Output contains zero total count
        /// - Output port StandardHandle is called with an empty vehicle list.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenNoVehiclesExistWithStatusShouldReturnEmptyList()
        {
            // Arrange
            var input = new GetVehiclesByStatusInput { Status = VehicleStatus.Available };

            _vehicleRepositoryMock
                .Setup(x => x.GetVehiclesByStatusAsync(VehicleStatus.Available, It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.GetVehiclesByStatusAsync(VehicleStatus.Available, It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.Is<GetVehiclesByStatusOutput>(o => o.TotalCount == 0)), Times.Once);
        }
    }
}
