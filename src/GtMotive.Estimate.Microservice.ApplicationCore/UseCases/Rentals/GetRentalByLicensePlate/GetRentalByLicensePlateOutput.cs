using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate
{
    /// <summary>
    /// Output for getting a rental by vehicle license plate.
    /// </summary>
    public sealed class GetRentalByLicensePlateOutput : IUseCaseOutput
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
        /// Gets or sets the vehicle brand.
        /// </summary>
        public string VehicleBrand { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle model.
        /// </summary>
        public string VehicleModel { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle license plate.
        /// </summary>
        public string VehicleLicensePlate { get; set; } = string.Empty;

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
        /// Gets or sets the rental notes.
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
