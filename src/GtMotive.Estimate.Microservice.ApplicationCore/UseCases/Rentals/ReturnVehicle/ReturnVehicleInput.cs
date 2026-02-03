using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle
{
    /// <summary>
    /// Input for the return vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets the unique identifier of the rental.
        /// </summary>
        public string RentalId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the return date of the vehicle.
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the current odometer reading (total kilometers) of the vehicle at return.
        /// </summary>
        public int CurrentKilometers { get; set; }

        /// <summary>
        /// Gets or sets additional notes about the return.
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
