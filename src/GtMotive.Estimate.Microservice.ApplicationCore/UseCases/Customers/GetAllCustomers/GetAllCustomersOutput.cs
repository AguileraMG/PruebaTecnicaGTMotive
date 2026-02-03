#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1010 // Opening square brackets should not be preceded by a space
#pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura

using System;
using System.Collections.ObjectModel;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers
{
    /// <summary>
    /// Output for getting all customers.
    /// </summary>
    public sealed class GetAllCustomersOutput : IUseCaseOutput
    {
        /// <summary>
        /// Gets or sets the list of customers.
        /// </summary>
        public Collection<CustomerDto> Customers { get; set; } = [];

        /// <summary>
        /// Gets or sets the total count of customers.
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Data transfer object for a customer.
    /// </summary>
    public sealed class CustomerDto
    {
        /// <summary>
        /// Gets or sets the customer identifier.
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
        /// Gets or sets the date and time when the customer was registered.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
