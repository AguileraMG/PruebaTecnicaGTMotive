#nullable enable
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1010 // Opening square brackets should not be preceded by a space
#pragma warning disable CA2227 // Las propiedades de colección deben ser de solo lectura

using System;
using System.Collections.ObjectModel;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus
{
    /// <summary>
    /// Output for the get vehicles by status use case.
    /// </summary>
    public sealed class GetVehiclesByStatusOutput : IUseCaseOutput
    {
        /// <summary>
        /// Gets or sets the list of vehicles matching the filter.
        /// </summary>
        public Collection<VehicleItem> Vehicles { get; set; } = [];

        /// <summary>
        /// Gets or sets the total count of vehicles returned.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the status filter that was applied (null means all statuses).
        /// </summary>
        public string? FilterApplied { get; set; }
    }

    /// <summary>
    /// Represents a single vehicle item in the list.
    /// </summary>
    public sealed class VehicleItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the vehicle.
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
        /// Gets or sets the date and time when the vehicle was added to the fleet.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}

#pragma warning restore SA1010
#pragma warning restore CA2227
#pragma warning restore SA1402
