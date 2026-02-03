using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus
{
    /// <summary>
    /// Input for getting vehicles filtered by status.
    /// </summary>
    public sealed class GetVehiclesByStatusInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets the vehicle status filter.
        /// If null, returns all vehicles regardless of status.
        /// </summary>
        public VehicleStatus? Status { get; set; }
    }
}
