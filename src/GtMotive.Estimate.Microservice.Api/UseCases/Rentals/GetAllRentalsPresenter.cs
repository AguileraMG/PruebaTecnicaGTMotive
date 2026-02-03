using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Rentals
{
    /// <summary>
    /// Presenter for the get all rentals use case.
    /// </summary>
    public sealed class GetAllRentalsPresenter
        : IGetAllRentalsOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful rentals retrieval.
        /// </summary>
        /// <param name="response">The output containing the rentals data.</param>
        public void StandardHandle(GetAllRentalsOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateOkResponse(response);
        }
    }
}
