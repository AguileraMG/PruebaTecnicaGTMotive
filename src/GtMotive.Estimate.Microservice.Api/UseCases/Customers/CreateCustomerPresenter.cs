using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Customers
{
    /// <summary>
    /// Presenter for the create customer use case.
    /// Transforms use case output into HTTP responses.
    /// </summary>
    public sealed class CreateCustomerPresenter
        : ICreateCustomerOutputPort,
          IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        public IActionResult ActionResult { get; private set; } = new OkResult();

        /// <summary>
        /// Handles successful customer creation.
        /// Maps the domain output to HTTP 201 Created response.
        /// </summary>
        /// <param name="response">The output containing the created customer data.</param>
        public void StandardHandle(CreateCustomerOutput response)
        {
            ActionResult = ((IWebApiPresenter)this).CreateCreatedResponse(response);
        }

        /// <summary>
        /// Handles business rule violations (email/license duplicate).
        /// Maps conflicts to HTTP 409 Conflict response.
        /// </summary>
        /// <param name="message">The error message describing the conflict.</param>
        public void ConflictHandle(string message)
        {
            ActionResult = ((IWebApiPresenter)this).CreateConflictProblem(message);
        }
    }
}
