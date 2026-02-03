using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    /// <summary>
    /// Presenter for the get vehicles by status use case.
    /// Transforms use case output into HTTP responses.
    /// </summary>
    public sealed class GetVehiclesByStatusPresenter
        : IGetVehiclesByStatusOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful retrieval of vehicles.
        /// Maps the domain output to HTTP 200 OK response.
        /// </summary>
        /// <param name="response">The output containing the list of vehicles.</param>
        public void StandardHandle(GetVehiclesByStatusOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateOkResponse(response);
        }
    }
}
