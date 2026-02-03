using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Customers
{
    /// <summary>
    /// Unit tests for the <see cref="GetAllCustomersUseCase"/> class.
    /// Tests validate retrieval of customer lists with proper data mapping
    /// and handling of empty result sets.
    /// </summary>
    public class GetAllCustomersUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IGetAllCustomersOutputPort> _outputPortMock;
        private readonly GetAllCustomersUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllCustomersUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public GetAllCustomersUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _outputPortMock = new Mock<IGetAllCustomersOutputPort>();
            _sut = new GetAllCustomersUseCase(_outputPortMock.Object, _customerRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that all customers are returned when customers exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates the successful retrieval of customer data:
        /// - Repository returns a list of 5 customers
        /// - Output contains the correct total count
        /// - Output port StandardHandle is called with the customer list.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenCustomersExistShouldReturnAllCustomers()
        {
            // Arrange
            var customers = EntityFakers.CustomerFaker.Generate(5);
            var input = new GetAllCustomersInput();

            _customerRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _customerRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<bool?>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.Is<GetAllCustomersOutput>(o => o.TotalCount == 5)), Times.Once);
        }

        /// <summary>
        /// Verifies that an empty list is returned when no customers exist in the system.
        /// </summary>
        /// <remarks>
        /// This test validates proper handling of empty result sets:
        /// - Repository returns an empty list
        /// - Output contains zero total count
        /// - Output port StandardHandle is called with an empty customer list.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenNoCustomersExistShouldReturnEmptyList()
        {
            // Arrange
            var input = new GetAllCustomersInput();

            _customerRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _customerRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<bool?>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.Is<GetAllCustomersOutput>(o => o.TotalCount == 0)), Times.Once);
        }
    }
}
