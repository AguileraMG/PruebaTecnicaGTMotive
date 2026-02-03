using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer
{
    /// <summary>
    /// Input for creating a new customer.
    /// </summary>
    public sealed class CreateCustomerInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets the full name of the customer.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the customer.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the customer.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the driver's license number of the customer.
        /// </summary>
        public string DriverLicenseNumber { get; set; } = string.Empty;
    }
}
