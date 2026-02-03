using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Rentals
{
    /// <summary>
    /// Presenter for the get rental by license plate use case.
    /// </summary>
    public sealed class GetRentalByLicensePlatePresenter
        : IGetRentalByLicensePlateOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful rental retrieval.
        /// </summary>
        /// <param name="response">The output containing the rental data.</param>
        public void StandardHandle(GetRentalByLicensePlateOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateOkResponse(response);
        }

        /// <summary>
        /// Handles bad request scenarios (invalid input data).
        /// </summary>
        /// <param name="message">The validation error message.</param>
        public void BadRequestHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateBadRequestProblem(message);
        }

        /// <summary>
        /// Handles not found scenarios (resource doesn't exist).
        /// </summary>
        /// <param name="message">The error message.</param>
        public void NotFoundHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateNotFoundProblem(message);
        }
    }
}
