using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.Customers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing customers.
    /// </summary>
#pragma warning disable S6960 // Controller actions with multiple responsibilities
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController(
        IUseCase<CreateCustomerInput> createCustomerUseCase,
        CreateCustomerPresenter createCustomerPresenter,
        IUseCase<GetAllCustomersInput> getAllCustomersUseCase,
        GetAllCustomersPresenter getAllCustomersPresenter) : ControllerBase
    {
        private readonly IUseCase<CreateCustomerInput> _createCustomerUseCase = createCustomerUseCase ?? throw new ArgumentNullException(nameof(createCustomerUseCase));
        private readonly CreateCustomerPresenter _createCustomerPresenter = createCustomerPresenter ?? throw new ArgumentNullException(nameof(createCustomerPresenter));
        private readonly IUseCase<GetAllCustomersInput> _getAllCustomersUseCase = getAllCustomersUseCase ?? throw new ArgumentNullException(nameof(getAllCustomersUseCase));
        private readonly GetAllCustomersPresenter _getAllCustomersPresenter = getAllCustomersPresenter ?? throw new ArgumentNullException(nameof(getAllCustomersPresenter));

        /// <summary>
        /// Creates a new customer in the system.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/customers
        ///     {
        ///         "name": "Juan Pérez",
        ///         "email": "juan@example.com",
        ///         "phoneNumber": "+34 600 123 456",
        ///         "driverLicenseNumber": "12345678Z"
        ///     }
        ///
        /// Business Rules:
        /// - Email must be unique in the system
        /// - New customers start with no active rentals.
        /// </remarks>
        /// <param name="input">The customer information to create.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The created customer details.</returns>
        /// <response code="201">Customer created successfully. Returns the created customer with its generated ID.</response>
        /// <response code="400">
        /// Invalid input data or business rule violation. Possible reasons:
        /// - Duplicate email address
        /// - Invalid data format.
        /// </response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateCustomerOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerInput input, CancellationToken ct)
        {
            await _createCustomerUseCase.ExecuteAsync(input, ct);
            return _createCustomerPresenter.ActionResult;
        }

        /// <summary>
        /// Gets all customers with optional filter by rental status.
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/customers                         // Get all customers
        ///     GET /api/customers?hasActiveRental=true    // Get only customers with active rentals
        ///     GET /api/customers?hasActiveRental=false   // Get only customers without active rentals
        ///
        /// Returns a list of customers with their information and rental status.
        /// </remarks>
        /// <param name="hasActiveRental">Optional filter by active rental status.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of customers.</returns>
        /// <response code="200">Customers retrieved successfully. Returns the list of customers.</response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetAllCustomersOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] bool? hasActiveRental, CancellationToken ct)
        {
            var input = new GetAllCustomersInput { HasActiveRental = hasActiveRental };
            await _getAllCustomersUseCase.ExecuteAsync(input, ct);
            return _getAllCustomersPresenter.ActionResult;
        }
    }
#pragma warning restore S6960
}
