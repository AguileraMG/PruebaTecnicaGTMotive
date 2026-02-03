using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Output for the create vehicle use case.
    /// </summary>
    public sealed class CreateVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Gets or sets the unique identifier of the created vehicle.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the brand of the vehicle.
        /// </summary>
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the model of the vehicle.
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manufacturing year of the vehicle.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the license plate of the vehicle.
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current kilometers driven by the vehicle.
        /// </summary>
        public int KilometersDriven { get; set; }

        /// <summary>
        /// Gets or sets the current status of the vehicle.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the vehicle was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
