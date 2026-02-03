using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Entities;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Unit tests for the <see cref="CreateVehicleUseCase"/> class.
    /// Tests validate vehicle creation including age eligibility requirements
    /// and business rule enforcement for fleet management.
    /// </summary>
    public class CreateVehicleUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<ICreateVehicleOutputPort> _outputPortMock;
        private readonly CreateVehicleUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVehicleUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public CreateVehicleUseCaseTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _outputPortMock = new Mock<ICreateVehicleOutputPort>();
            _sut = new CreateVehicleUseCase(_outputPortMock.Object, _vehicleRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that a vehicle is created successfully when it meets the age eligibility requirement.
        /// </summary>
        /// <remarks>
        /// This test validates the happy path for vehicle registration:
        /// - Vehicle is less than 5 years old
        /// - Vehicle repository AddAsync is called once
        /// - Output port StandardHandle is called with the created vehicle details.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleIsEligibleShouldCreateVehicleSuccessfully()
        {
            // Arrange
            var currentYear = System.DateTime.UtcNow.Year;
            var input = new CreateVehicleInput
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = currentYear - 2,
                LicensePlate = "ABC-1234",
                KilometersDriven = 10000
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByLicensePlateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Vehicle)null);

            _vehicleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no vehicle is created when it exceeds the maximum age limit of 5 years.
        /// </summary>
        /// <remarks>
        /// This test validates the business rule that vehicles older than 5 years cannot be added to the fleet:
        /// - Vehicle year is older than 5 years from current year
        /// - Vehicle repository AddAsync is never called
        /// - Output port NotFoundHandle is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleIsTooOldShouldNotCreateVehicle()
        {
            // Arrange
            var input = new CreateVehicleInput
            {
                Brand = "Old Brand",
                Model = "Old Model",
                Year = 2015,
                LicensePlate = "ABC-1234",
                KilometersDriven = 100000
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByLicensePlateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Vehicle)null);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.BadRequestHandle(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no vehicle is created when license plate already exists.
        /// </summary>
        /// <remarks>
        /// This test checks the conflict handling when a duplicate license plate is provided:
        /// - Vehicle with the same license plate exists
        /// - Vehicle repository AddAsync is never called
        /// - Output port ConflictHandle is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenLicensePlateExistsShouldNotCreateVehicle()
        {
            // Arrange
            var currentYear = System.DateTime.UtcNow.Year;
            var existingVehicle = Vehicle.Create("Toyota", "Corolla", currentYear - 1, "ABC-1234", 5000);

            var input = new CreateVehicleInput
            {
                Brand = "Honda",
                Model = "Civic",
                Year = currentYear - 2,
                LicensePlate = "ABC-1234",
                KilometersDriven = 10000
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByLicensePlateAsync("ABC-1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingVehicle);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _vehicleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
        }
    }
}
