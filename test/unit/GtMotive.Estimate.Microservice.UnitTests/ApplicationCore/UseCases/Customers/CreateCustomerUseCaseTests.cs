using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.UseCases.Customers
{
    /// <summary>
    /// Unit tests for the <see cref="CreateCustomerUseCase"/> class.
    /// Tests validate customer registration including uniqueness constraints
    /// and proper handling of duplicate email addresses.
    /// </summary>
    public class CreateCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ICreateCustomerOutputPort> _outputPortMock;
        private readonly CreateCustomerUseCase _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCustomerUseCaseTests"/> class.
        /// Sets up all mock dependencies and the system under test (SUT).
        /// </summary>
        public CreateCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _outputPortMock = new Mock<ICreateCustomerOutputPort>();
            _sut = new CreateCustomerUseCase(_outputPortMock.Object, _customerRepositoryMock.Object);
        }

        /// <summary>
        /// Verifies that a customer is created successfully when no customer with the same email exists.
        /// </summary>
        /// <remarks>
        /// This test validates the happy path scenario for customer registration:
        /// - Email is unique in the system
        /// - Customer repository AddAsync is called once
        /// - Output port StandardHandle is called with the created customer details.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenCustomerDoesNotExistShouldCreateCustomerSuccessfully()
        {
            // Arrange
            var customer = EntityFakers.CustomerFaker.Generate();
            var input = new CreateCustomerInput
            {
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                DriverLicenseNumber = customer.DriverLicenseNumber
            };

            _customerRepositoryMock
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer)null);

            _customerRepositoryMock
                .Setup(x => x.GetByDriverLicenseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer)null);

            _customerRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
            _outputPortMock.Verify(x => x.StandardHandle(It.IsAny<CreateCustomerOutput>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no customer is created when a customer with the same email already exists.
        /// </summary>
        /// <remarks>
        /// This test validates the business rule that email addresses must be unique:
        /// - An existing customer with the same email is found
        /// - Customer repository AddAsync is never called
        /// - Output port NotFoundHandle is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenCustomerEmailExistsShouldNotCreateCustomer()
        {
            // Arrange
            var existingCustomer = EntityFakers.CustomerFaker.Generate();
            var input = new CreateCustomerInput
            {
                Name = "New Customer",
                Email = existingCustomer.Email,
                PhoneNumber = "123456789",
                DriverLicenseNumber = "NEW123"
            };

            _customerRepositoryMock
                .Setup(x => x.GetByEmailAsync(existingCustomer.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCustomer);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies that no customer is created when a customer with the same driver license already exists.
        /// </summary>
        /// <remarks>
        /// This test validates the business rule that driver license numbers must be unique:
        /// - An existing customer with the same driver license is found
        /// - Customer repository AddAsync is never called
        /// - Output port ConflictHandle is called with an appropriate error message.
        /// </remarks>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteAsyncWhenCustomerDriverLicenseExistsShouldNotCreateCustomer()
        {
            // Arrange
            var existingCustomer = EntityFakers.CustomerFaker.Generate();
            var input = new CreateCustomerInput
            {
                Name = "New Customer",
                Email = "newemail@example.com",
                PhoneNumber = "123456789",
                DriverLicenseNumber = existingCustomer.DriverLicenseNumber
            };

            _customerRepositoryMock
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer)null);

            _customerRepositoryMock
                .Setup(x => x.GetByDriverLicenseAsync(existingCustomer.DriverLicenseNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCustomer);

            // Act
            await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
            _outputPortMock.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
        }
    }
}
