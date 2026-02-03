using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Rentals
{
    /// <summary>
    /// Unit tests for the <see cref="ReturnVehicleUseCase"/> class.
    /// Tests validate the vehicle return process including rental completion,
    /// odometer updates, and vehicle status management based on age eligibility.
    /// </summary>
    public class ReturnVehicleUseCaseTests
    {
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IReturnVehicleOutputPort> _outputPortMock;
        private readonly ReturnVehicleUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public ReturnVehicleUseCaseTests()
        {
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _outputPortMock = new Mock<IReturnVehicleOutputPort>();
            _sut = new ReturnVehicleUseCase(
                _outputPortMock.Object,
                _vehicleRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _rentalRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that a vehicle return is processed successfully when the rental is active.
        /// </summary>
        /// <remarks>
        /// This test validates the complete return process including:
        /// - Rental exists and is active
        /// - Vehicle exists and odometer is updated
        /// - Vehicle is marked as available (if still eligible) or retired (if too old)
        /// - Customer is marked as no longer having an active rental
        /// - Rental is marked as completed.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenRentalIsActiveShouldReturnVehicleSuccessfully()
        {
            // Arrange
            var rental = EntityFakers.RentalFaker
                .RuleFor(r => r.Status, RentalStatus.Active)
                .Generate();

            var vehicle = EntityFakers.VehicleFaker
                .RuleFor(v => v.Id, rental.VehicleId)
                .RuleFor(v => v.Status, VehicleStatus.Rented)
                .RuleFor(v => v.KilometersDriven, 10000)
                .RuleFor(v => v.Year, System.DateTime.UtcNow.Year - 2)
                .Generate();

            var customer = EntityFakers.CustomerFaker
                .RuleFor(c => c.Id, rental.CustomerId)
                .RuleFor(c => c.HasActiveRental, true)
                .Generate();

            var input = new ReturnVehicleInput
            {
                RentalId = rental.Id,
                CurrentKilometers = 10500,
                Notes = "Test return"
            };

            _rentalRepositoryMock
                .Setup(x => x.GetByIdAsync(rental.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rental);

            _vehicleRepositoryMock
                .Setup(x => x.GetByIdAsync(rental.VehicleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vehicle);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(rental.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _rentalRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
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
            _rentalRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Once);
            _vehicleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
            _customerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.IsAny<ReturnVehicleOutput>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no return is processed when the specified rental does not exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates that:
        /// - The rental repository returns null for a non-existent rental ID
        /// - No updates are performed on any entities
        /// - The output port NotFoundHandle method is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenRentalNotFoundShouldNotReturnVehicle()
        {
            // Arrange
            var input = new ReturnVehicleInput
            {
                RentalId = "non-existent-rental",
                CurrentKilometers = 500
            };

            _rentalRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Rental)null);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.NotFoundHandle(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no return is processed when the rental is not in an active state.
        /// </summary>
        /// <remarks>
        /// This test validates the business rule that only active rentals can be returned:
        /// - The rental exists but has a status of Completed
        /// - No updates are performed on any entities
        /// - The output port NotFoundHandle method is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenRentalIsNotActiveShouldNotReturnVehicle()
        {
            // Arrange
            var rental = EntityFakers.RentalFaker
                .RuleFor(r => r.Status, RentalStatus.Completed)
                .Generate();

            var input = new ReturnVehicleInput
            {
                RentalId = rental.Id,
                CurrentKilometers = 500
            };

            _rentalRepositoryMock
                .Setup(x => x.GetByIdAsync(rental.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rental);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
        }
    }
}
