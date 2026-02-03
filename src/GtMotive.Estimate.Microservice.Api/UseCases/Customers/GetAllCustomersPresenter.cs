using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Customers
{
    /// <summary>
    /// Presenter for the get all customers use case.
    /// </summary>
    public sealed class GetAllCustomersPresenter
        : IGetAllCustomersOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful customers retrieval.
        /// </summary>
        /// <param name="response">The output containing the customers data.</param>
        public void StandardHandle(GetAllCustomersOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateOkResponse(response);
        }
    }
}
