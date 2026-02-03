using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Rentals
{
    /// <summary>
    /// Presenter for the rent vehicle use case.
    /// Transforms use case output into HTTP responses.
    /// </summary>
    public sealed class RentVehiclePresenter
        : IRentVehicleOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful rental creation.
        /// Maps the domain output to HTTP 201 Created response.
        /// </summary>
        /// <param name="response">The output containing the created rental data.</param>
        public void StandardHandle(RentVehicleOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateCreatedResponse(response);
        }

        /// <summary>
        /// Handles validation errors (invalid dates, vehicle too old).
        /// Maps errors to HTTP 400 Bad Request response.
        /// </summary>
        /// <param name="message">The validation error message.</param>
        public void BadRequestHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateBadRequestProblem(message);
        }

        /// <summary>
        /// Handles not found scenarios (vehicle or customer doesn't exist).
        /// Maps errors to HTTP 404 Not Found response.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void NotFoundHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateNotFoundProblem(message);
        }

        /// <summary>
        /// Handles business rule violations (vehicle not available, customer has active rental).
        /// Maps errors to HTTP 409 Conflict response.
        /// </summary>
        /// <param name="message">The business rule violation message.</param>
        public void ConflictHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateConflictProblem(message);
        }
    }
}
