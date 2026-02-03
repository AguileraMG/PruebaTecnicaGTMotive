using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.Rentals;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing vehicle rentals.
    /// </summary>
#pragma warning disable S6960 // Controller actions with multiple responsibilities
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
#pragma warning disable S107 // Methods should not have too many parameters
    public class RentalsController(
        IUseCase<RentVehicleInput> rentVehicleUseCase,
        IUseCase<ReturnVehicleInput> returnVehicleUseCase,
        IUseCase<GetRentalByLicensePlateInput> getRentalByLicensePlateUseCase,
        IUseCase<GetAllRentalsInput> getAllRentalsUseCase,
        RentVehiclePresenter rentVehiclePresenter,
        ReturnVehiclePresenter returnVehiclePresenter,
        GetRentalByLicensePlatePresenter getRentalByLicensePlatePresenter,
        GetAllRentalsPresenter getAllRentalsPresenter) : ControllerBase
#pragma warning restore S107 // Methods should not have too many parameters
    {
        private readonly IUseCase<RentVehicleInput> _rentVehicleUseCase = rentVehicleUseCase ?? throw new ArgumentNullException(nameof(rentVehicleUseCase));
        private readonly RentVehiclePresenter _rentVehiclePresenter = rentVehiclePresenter ?? throw new ArgumentNullException(nameof(rentVehiclePresenter));
        private readonly IUseCase<ReturnVehicleInput> _returnVehicleUseCase = returnVehicleUseCase ?? throw new ArgumentNullException(nameof(returnVehicleUseCase));
        private readonly ReturnVehiclePresenter _returnVehiclePresenter = returnVehiclePresenter ?? throw new ArgumentNullException(nameof(returnVehiclePresenter));
        private readonly IUseCase<GetRentalByLicensePlateInput> _getRentalByLicensePlateUseCase = getRentalByLicensePlateUseCase ?? throw new ArgumentNullException(nameof(getRentalByLicensePlateUseCase));
        private readonly GetRentalByLicensePlatePresenter _getRentalByLicensePlatePresenter = getRentalByLicensePlatePresenter ?? throw new ArgumentNullException(nameof(getRentalByLicensePlatePresenter));
        private readonly IUseCase<GetAllRentalsInput> _getAllRentalsUseCase = getAllRentalsUseCase ?? throw new ArgumentNullException(nameof(getAllRentalsUseCase));
        private readonly GetAllRentalsPresenter _getAllRentalsPresenter = getAllRentalsPresenter ?? throw new ArgumentNullException(nameof(getAllRentalsPresenter));

        /// <summary>
        /// Rents a vehicle to a customer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/rentals/rentvehicle
        ///     {
        ///         "vehicleId": "507f1f77bcf86cd799439011",
        ///         "customerId": "507f191e810c19729de860ea",
        ///         "expectedReturnDate": "2024-02-01T10:00:00Z",
        ///         "notes": "Optional notes"
        ///     }
        ///
        /// Business Rules:
        /// - Vehicle must exist and be available
        /// - Vehicle must not be older than 5 years
        /// - Customer must exist and not have an active rental
        /// - Expected return date must be in the future.
        /// </remarks>
        /// <param name="input">The rental information.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The created rental details.</returns>
        /// <response code="201">Rental created successfully. Returns the rental information.</response>
        /// <response code="400">
        /// Invalid input data or business rule violation. Possible reasons:
        /// - Vehicle not found or not available
        /// - Customer not found or already has active rental
        /// - Invalid dates
        /// - Vehicle too old.
        /// </response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RentVehicleOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RentVehicle([FromBody] RentVehicleInput input, CancellationToken ct)
        {
            await _rentVehicleUseCase.ExecuteAsync(input, ct);
            return _rentVehiclePresenter.ActionResult;
        }

        /// <summary>
        /// Returns a rented vehicle.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/rentals/returnvehicle
        ///     {
        ///         "rentalId": "507f1f77bcf86cd799439011",
        ///         "returnDate": "2024-01-20T10:00:00Z",
        ///         "currentKilometers": 45150,
        ///         "notes": "Vehicle returned in good condition"
        ///     }
        ///
        /// Business Rules:
        /// - Rental must exist and be active
        /// - Vehicle, customer, and rental must exist in the system
        /// - Current kilometers must be equal or greater than vehicle's current odometer reading
        /// - Customer will be marked as not having an active rental
        /// - Vehicle odometer will be updated to the provided value
        /// - Vehicle will be marked as available if eligible for fleet (less than 5 years old), otherwise retired.
        /// </remarks>
        /// <param name="input">The return vehicle information.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The result of the return vehicle operation.</returns>
        /// <response code="200">Vehicle returned successfully. Returns the rental completion details.</response>
        /// <response code="400">
        /// Invalid input data or business rule violation. Possible reasons:
        /// - Rental not found or not active
        /// - Vehicle or customer not found
        /// - Invalid odometer reading (less than current reading)
        /// - Rental already completed or cancelled.
        /// </response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ReturnVehicleOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReturnVehicle([FromBody] ReturnVehicleInput input, CancellationToken ct)
        {
            await _returnVehicleUseCase.ExecuteAsync(input, ct);
            return _returnVehiclePresenter.ActionResult;
        }

        /// <summary>
        /// Gets an active rental by vehicle license plate.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/rentals/getbylicenseplate?licensePlate=ABC123
        ///
        /// Returns the active rental information for the specified vehicle license plate.
        /// </remarks>
        /// <param name="licensePlate">The vehicle license plate.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The active rental information.</returns>
        /// <response code="200">Rental found successfully. Returns the rental information.</response>
        /// <response code="404">
        /// Not found. Possible reasons:
        /// - Vehicle with the specified license plate not found
        /// - No active rental found for the vehicle.
        /// </response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetRentalByLicensePlateOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLicensePlate([FromQuery] string licensePlate, CancellationToken ct)
        {
            var input = new GetRentalByLicensePlateInput { LicensePlate = licensePlate };
            await _getRentalByLicensePlateUseCase.ExecuteAsync(input, ct);
            return _getRentalByLicensePlatePresenter.ActionResult;
        }

        /// <summary>
        /// Gets all rentals with optional status filter.
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/rentals/getall                  // Get all rentals (active and completed)
        ///     GET /api/rentals/getall?status=0         // Get only active rentals
        ///     GET /api/rentals/getall?status=1         // Get only completed rentals
        ///
        /// Rental Status Values:
        /// - 0: Active
        /// - 1: Completed
        ///
        /// Returns a list of rentals with vehicle and customer information.
        /// </remarks>
        /// <param name="status">Optional rental status filter.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of rentals.</returns>
        /// <response code="200">Rentals retrieved successfully. Returns the list of rentals.</response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetAllRentalsOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] RentalStatus? status, CancellationToken ct)
        {
            var input = new GetAllRentalsInput { Status = status };
            await _getAllRentalsUseCase.ExecuteAsync(input, ct);
            return _getAllRentalsPresenter.ActionResult;
        }
    }
}
