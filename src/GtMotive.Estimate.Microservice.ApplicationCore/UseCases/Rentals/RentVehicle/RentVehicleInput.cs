using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle
{
    /// <summary>
    /// Input for renting a vehicle to a customer.
    /// </summary>
    public sealed class RentVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets the vehicle identifier to rent.
        /// </summary>
        public string VehicleId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer identifier who is renting the vehicle.
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expected return date for the rental.
        /// </summary>
        public DateTime ExpectedReturnDate { get; set; }

        /// <summary>
        /// Gets or sets optional notes for the rental.
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
