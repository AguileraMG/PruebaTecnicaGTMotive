using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Rentals
{
    /// <summary>
    /// Unit tests for the <see cref="GetRentalByLicensePlateUseCase"/> class.
    /// Tests validate rental lookup by vehicle license plate, including
    /// vehicle existence checks and active rental verification.
    /// </summary>
    public class GetRentalByLicensePlateUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IGetRentalByLicensePlateOutputPort> _outputPortMock;
        private readonly GetRentalByLicensePlateUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRentalByLicensePlateUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public GetRentalByLicensePlateUseCaseTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _outputPortMock = new Mock<IGetRentalByLicensePlateOutputPort>();
            _sut = new GetRentalByLicensePlateUseCase(
                _outputPortMock.Object,
                _vehicleRepositoryMock.Object,
                _rentalRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that the active rental is returned when a vehicle with the specified license plate
        /// exists and has an active rental.
        /// </summary>
        /// <remarks>
        /// This test validates the successful rental lookup scenario:
        /// - Vehicle is found by license plate
        /// - Vehicle has an active rental
        /// - Rental details are returned including vehicle and customer information
        /// - Output port StandardHandle is called with the rental data.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleExistsAndHasActiveRentalShouldReturnRental()
        {
            // Arrange
            var vehicle = EntityFakers.VehicleFaker
                .RuleFor(v => v.Status, VehicleStatus.Rented)
                .Generate();

            var rental = EntityFakers.RentalFaker
                .RuleFor(r => r.VehicleId, vehicle.Id)
                .RuleFor(r => r.Status, RentalStatus.Active)
                .Generate();

            var input = new GetRentalByLicensePlateInput
            {
                LicensePlate = vehicle.LicensePlate
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByLicensePlateAsync(vehicle.LicensePlate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _rentalRepositoryMock
                .Setup(x => x.GetActiveRentalByVehicleIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rental);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.GetByLicensePlateAsync(vehicle.LicensePlate, It.IsAny<CancellationToken>()), Times.Once);
            _rentalRepositoryMock.Verify(x => x.GetActiveRentalByVehicleIdAsync(vehicle.Id, It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.IsAny<GetRentalByLicensePlateOutput>()), Times.Once);
        }

        /// <summary>
        /// Verifies that a not found error is returned when no vehicle with the specified license plate exists.
        /// </summary>
        /// <remarks>
        /// This test validates error handling for non-existent vehicles:
        /// - Vehicle repository returns null for the license plate
        /// - No rental lookup is performed
        /// - Output port NotFoundHandle is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleNotFoundShouldReturnNotFound()
        {
            // Arrange
            var input = new GetRentalByLicensePlateInput
            {
                LicensePlate = "ABC-1234"
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByLicensePlateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Vehicle)null);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.GetByLicensePlateAsync(input.LicensePlate, It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.NotFoundHandle(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies that a not found error is returned when the vehicle exists but has no active rental.
        /// </summary>
        /// <remarks>
        /// This test validates handling of vehicles without active rentals:
        /// - Vehicle is found by license plate
        /// - Rental repository returns null for active rental lookup
        /// - Output port NotFoundHandle is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleHasNoActiveRentalShouldReturnNotFound()
        {
            // Arrange
            var vehicle = EntityFakers.VehicleFaker
                .RuleFor(v => v.Status, VehicleStatus.Available)
                .Generate();

            var input = new GetRentalByLicensePlateInput
            {
                LicensePlate = vehicle.LicensePlate
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByLicensePlateAsync(vehicle.LicensePlate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _rentalRepositoryMock
                .Setup(x => x.GetActiveRentalByVehicleIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Rental)null);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.GetByLicensePlateAsync(vehicle.LicensePlate, It.IsAny<CancellationToken>()), Times.Once);
            _rentalRepositoryMock.Verify(x => x.GetActiveRentalByVehicleIdAsync(vehicle.Id, It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.NotFoundHandle(It.IsAny<string>()), Times.Once);
        }
    }
}
