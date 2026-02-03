using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer
{
    /// <summary>
    /// Output for the create customer use case.
    /// </summary>
    public sealed class CreateCustomerOutput : IUseCaseOutput
    {
        /// <summary>
        /// Gets or sets the unique identifier of the created customer.
        /// </summary>
        public string Id { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets or sets a value indicating whether the customer has an active rental.
        /// </summary>
        public bool HasActiveRental { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the customer was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
