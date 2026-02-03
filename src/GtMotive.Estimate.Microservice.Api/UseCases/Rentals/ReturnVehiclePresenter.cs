using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Rentals
{
    /// <summary>
    /// Presenter for the return vehicle use case.
    /// Transforms use case output into HTTP responses.
    /// </summary>
    public sealed class ReturnVehiclePresenter
        : IReturnVehicleOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful vehicle return.
        /// Maps the domain output to HTTP 200 OK response.
        /// </summary>
        /// <param name="response">The output containing the return rental data.</param>
        public void StandardHandle(ReturnVehicleOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateOkResponse(response);
        }

        /// <summary>
        /// Handles validation errors (negative kilometers, invalid odometer).
        /// Maps errors to HTTP 400 Bad Request response.
        /// </summary>
        /// <param name="message">The validation error message.</param>
        public void BadRequestHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateBadRequestProblem(message);
        }

        /// <summary>
        /// Handles not found scenarios (rental, vehicle, or customer doesn't exist).
        /// Maps errors to HTTP 404 Not Found response.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void NotFoundHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateNotFoundProblem(message);
        }

        /// <summary>
        /// Handles business rule violations (rental not active).
        /// Maps errors to HTTP 409 Conflict response.
        /// </summary>
        /// <param name="message">The business rule violation message.</param>
        public void ConflictHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateConflictProblem(message);
        }
    }
}
