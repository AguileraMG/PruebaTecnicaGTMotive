using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers
{
    /// <summary>
    /// Input for getting all customers.
    /// </summary>
    public sealed class GetAllCustomersInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets whether to include only customers with active rentals.
        /// If null, returns all customers.
        /// </summary>
        public bool? HasActiveRental { get; set; }
    }
}
