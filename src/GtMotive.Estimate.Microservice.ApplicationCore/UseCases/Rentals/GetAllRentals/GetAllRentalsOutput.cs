#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1010 // Opening square brackets should not be preceded by a space
#pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura

using System;
using System.Collections.ObjectModel;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals
{
    /// <summary>
    /// Output for getting all rentals.
    /// </summary>
    public sealed class GetAllRentalsOutput : IUseCaseOutput
    {
        /// <summary>
        /// Gets or sets the list of rentals.
        /// </summary>
        public Collection<RentalDto> Rentals { get; set; } = [];

        /// <summary>
        /// Gets or sets the total count of rentals.
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Represents a rental data transfer object.
    /// </summary>
    public sealed class RentalDto
    {
        /// <summary>
        /// Gets or sets the rental identifier.
        /// </summary>
        public string RentalId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
        public string VehicleId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rental start date.
        /// </summary>
        public DateTime RentalDate { get; set; }

        /// <summary>
        /// Gets or sets the actual return date.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the expected return date.
        /// </summary>
        public DateTime ExpectedReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the rental status.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the rental is overdue.
        /// </summary>
        public bool IsOverdue { get; set; }
    }
}
