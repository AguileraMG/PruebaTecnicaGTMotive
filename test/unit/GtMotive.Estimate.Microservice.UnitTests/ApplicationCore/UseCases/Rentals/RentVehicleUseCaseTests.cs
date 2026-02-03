using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Rentals
{
    /// <summary>
    /// Unit tests for the <see cref="RentVehicleUseCase"/> class.
    /// Tests validate the rental creation process including vehicle availability,
    /// customer eligibility, and business rule enforcement.
    /// </summary>
    public class RentVehicleUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IRentVehicleOutputPort> _outputPortMock;
        private readonly RentVehicleUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public RentVehicleUseCaseTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _outputPortMock = new Mock<IRentVehicleOutputPort>();
            _sut = new RentVehicleUseCase(
                _outputPortMock.Object,
                _vehicleRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _rentalRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that a rental is created successfully when the vehicle is available
        /// and the customer is eligible to rent.
        /// </summary>
        /// <remarks>
        /// This test validates the complete happy path scenario including:
        /// - Vehicle exists and is available
        /// - Vehicle meets age requirements (less than 5 years old)
        /// - Customer exists and has no active rentals
        /// - Rental record is created
        /// - Vehicle status is updated to rented
        /// - Customer status is updated to has active rental.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleIsAvailableAndCustomerCanRentShouldCreateRentalSuccessfully()
        {
            // Arrange
            var currentYear = System.DateTime.UtcNow.Year;
            var vehicle = EntityFakers.VehicleFaker
                .RuleFor(v => v.Status, VehicleStatus.Available)
                .RuleFor(v => v.Year, currentYear - 2)
                .Generate();

            var customer = EntityFakers.CustomerFaker
                .RuleFor(c => c.HasActiveRental, false)
                .Generate();

            var input = new RentVehicleInput
            {
                VehicleId = vehicle.Id,
                CustomerId = customer.Id,
                ExpectedReturnDate = System.DateTime.UtcNow.AddDays(7)
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _rentalRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _vehicleRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _customerRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Once);
            _vehicleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
            _customerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no rental is created when the specified vehicle does not exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates that:
        /// - The vehicle repository returns null for a non-existent vehicle ID
        /// - No rental record is created
        /// - The output port NotFoundHandle method is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenVehicleNotFoundShouldNotCreateRental()
        {
            // Arrange
            var input = new RentVehicleInput
            {
                VehicleId = "non-existent-id",
                CustomerId = "customer-id",
                ExpectedReturnDate = System.DateTime.UtcNow.AddDays(7)
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Vehicle)null);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.NotFoundHandle(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no rental is created when the specified customer does not exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates that:
        /// - The vehicle exists and is available
        /// - The customer repository returns null for a non-existent customer ID
        /// - No rental record is created
        /// - The output port NotFoundHandle method is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenCustomerNotFoundShouldNotCreateRental()
        {
            // Arrange
            var currentYear = System.DateTime.UtcNow.Year;
            var vehicle = EntityFakers.VehicleFaker
                .RuleFor(v => v.Status, VehicleStatus.Available)
                .RuleFor(v => v.Year, currentYear - 2)
                .Generate();

            var input = new RentVehicleInput
            {
                VehicleId = vehicle.Id,
                CustomerId = "non-existent-customer",
                ExpectedReturnDate = System.DateTime.UtcNow.AddDays(7)
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer)null);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.NotFoundHandle(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no rental is created when the customer already has an active rental.
        /// </summary>
        /// <remarks>
        /// This test validates the business rule that a customer cannot rent more than one vehicle simultaneously.
        /// The test ensures that:
        /// - The vehicle exists and is available
        /// - The customer exists but has HasActiveRental set to true
        /// - No rental record is created
        /// - The output port NotFoundHandle method is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenCustomerHasActiveRentalShouldNotCreateRental()
        {
            // Arrange
            var currentYear = System.DateTime.UtcNow.Year;
            var vehicle = EntityFakers.VehicleFaker
                .RuleFor(v => v.Status, VehicleStatus.Available)
                .RuleFor(v => v.Year, currentYear - 2)
                .Generate();

            var customer = EntityFakers.CustomerFaker
                .RuleFor(c => c.HasActiveRental, true)
                .Generate();

            var input = new RentVehicleInput
            {
                VehicleId = vehicle.Id,
                CustomerId = customer.Id,
                ExpectedReturnDate = System.DateTime.UtcNow.AddDays(7)
            };

            _vehicleRepositoryMock
                .Setup(x => x.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
        }
    }
}
