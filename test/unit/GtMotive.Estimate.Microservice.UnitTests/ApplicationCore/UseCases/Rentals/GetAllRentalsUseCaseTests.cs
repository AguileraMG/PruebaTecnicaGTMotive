using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Rentals
{
    /// <summary>
    /// Unit tests for the <see cref="GetAllRentalsUseCase"/> class.
    /// Tests validate rental retrieval with optional status filtering,
    /// proper data mapping, and handling of empty result sets.
    /// </summary>
    public class GetAllRentalsUseCaseTests
    {
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IGetAllRentalsOutputPort> _outputPortMock;
        private readonly GetAllRentalsUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllRentalsUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public GetAllRentalsUseCaseTests()
        {
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _outputPortMock = new Mock<IGetAllRentalsOutputPort>();
            _sut = new GetAllRentalsUseCase(_outputPortMock.Object, _rentalRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that all rentals are returned when rentals exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates the successful retrieval of rental data:
        /// - Repository returns a list of 5 rentals
        /// - Output contains the correct total count
        /// - Output port StandardHandle is called with the rental list.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenRentalsExistShouldReturnAllRentals()
        {
            // Arrange
            var rentals = EntityFakers.RentalFaker.Generate(5);
            var input = new GetAllRentalsInput();

            _rentalRepositoryMock
                .Setup(x => x.GetAllRentalsAsync(It.IsAny<RentalStatus?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rentals);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.GetAllRentalsAsync(It.IsAny<RentalStatus?>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.Is<GetAllRentalsOutput>(o => o.TotalCount == 5)), Times.Once);
        }

        /// <summary>
        /// Verifies that an empty list is returned when no rentals exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates proper handling of empty result sets:
        /// - Repository returns an empty list
        /// - Output contains zero total count
        /// - Output port StandardHandle is called with an empty rental list.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenNoRentalsExistShouldReturnEmptyList()
        {
            // Arrange
            var input = new GetAllRentalsInput();

            _rentalRepositoryMock
                .Setup(x => x.GetAllRentalsAsync(It.IsAny<RentalStatus?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _rentalRepositoryMock.Verify(x => x.GetAllRentalsAsync(It.IsAny<RentalStatus?>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.Is<GetAllRentalsOutput>(o => o.TotalCount == 0)), Times.Once);
        }
    }
}
