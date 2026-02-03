using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus;
using GtMotive.Estimate.Microservice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing vehicles in the fleet.
    /// </summary>
#pragma warning disable S6960 // Controller actions with multiple responsibilities
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VehiclesController(
        IUseCase<CreateVehicleInput> createVehicleUseCase,
        CreateVehiclePresenter createVehiclePresenter,
        IUseCase<GetVehiclesByStatusInput> getVehiclesByStatusUseCase,
        GetVehiclesByStatusPresenter getVehiclesByStatusPresenter) : ControllerBase
    {
        private readonly IUseCase<CreateVehicleInput> _createVehicleUseCase = createVehicleUseCase ?? throw new ArgumentNullException(nameof(createVehicleUseCase));
        private readonly CreateVehiclePresenter _createVehiclePresenter = createVehiclePresenter ?? throw new ArgumentNullException(nameof(createVehiclePresenter));
        private readonly IUseCase<GetVehiclesByStatusInput> _getVehiclesByStatusUseCase = getVehiclesByStatusUseCase ?? throw new ArgumentNullException(nameof(getVehiclesByStatusUseCase));
        private readonly GetVehiclesByStatusPresenter _getVehiclesByStatusPresenter = getVehiclesByStatusPresenter ?? throw new ArgumentNullException(nameof(getVehiclesByStatusPresenter));

        /// <summary>
        /// Creates a new vehicle in the fleet.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/vehicles
        ///     {
        ///         "brand": "Toyota",
        ///         "model": "Corolla",
        ///         "year": 2023,
        ///         "licensePlate": "ABC123",
        ///         "kilometersDriven": 45000
        ///     }
        ///
        /// Business Rules:
        /// - Vehicle must not be older than 5 years
        /// - License plate must be unique
        /// - Kilometers driven must be 0 or positive.
        /// </remarks>
        /// <param name="input">The vehicle information to create.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The created vehicle details.</returns>
        /// <response code="201">Vehicle created successfully. Returns the created vehicle with its generated ID.</response>
        /// <response code="400">
        /// Invalid input data or business rule violation. Possible reasons:
        /// - Vehicle too old (more than 5 years)
        /// - Duplicate license plate
        /// - Invalid data format.
        /// </response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateVehicleOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleInput input, CancellationToken ct)
        {
            // 1. Ejecutar el caso de uso (lógica de negocio)
            await _createVehicleUseCase.ExecuteAsync(input, ct);
            return _createVehiclePresenter.ActionResult;
        }

        /// <summary>
        /// Gets vehicles filtered by status.
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/vehicles?status=Available     // Get only available vehicles
        ///     GET /api/vehicles?status=Rented        // Get only rented vehicles
        ///     GET /api/vehicles?status=Retired       // Get only retired vehicles
        ///     GET /api/vehicles                      // Get all vehicles
        ///
        /// Valid status values: Available (0), Rented(1), Retired(2).
        /// </remarks>
        /// <param name="status">Optional status filter. If not provided, returns all vehicles.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of vehicles matching the filter.</returns>
        /// <response code="200">Successfully retrieved the list of vehicles.</response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="500">Internal server error occurred while processing the request.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GetVehiclesByStatusOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVehiclesByStatus([FromQuery] VehicleStatus? status, CancellationToken ct)
        {
            var input = new GetVehiclesByStatusInput { Status = status };
            await _getVehiclesByStatusUseCase.ExecuteAsync(input, ct);
            return _getVehiclesByStatusPresenter.ActionResult;
        }
    }
#pragma warning restore S6960
}
