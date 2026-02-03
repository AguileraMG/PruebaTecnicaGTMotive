using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    /// <summary>
    /// Presenter for the create vehicle use case.
    /// Transforms use case output into HTTP responses.
    /// </summary>
    public sealed class CreateVehiclePresenter
        : ICreateVehicleOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful vehicle creation.
        /// Maps the domain output to HTTP 201 Created response.
        /// </summary>
        /// <param name="response">The output containing the created vehicle data.</param>
        public void StandardHandle(CreateVehicleOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateCreatedResponse(response);
        }

        /// <summary>
        /// Handles validation errors (vehicle too old, invalid data).
        /// Maps errors to HTTP 400 Bad Request response.
        /// </summary>
        /// <param name="message">The validation error message.</param>
        public void BadRequestHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateBadRequestProblem(message);
        }

        /// <summary>
        /// Handles business rule conflicts (license plate duplicate).
        /// Maps conflicts to HTTP 409 Conflict response.
        /// </summary>
        /// <param name="message">The conflict error message.</param>
        public void ConflictHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateConflictProblem(message);
        }
    }
}
