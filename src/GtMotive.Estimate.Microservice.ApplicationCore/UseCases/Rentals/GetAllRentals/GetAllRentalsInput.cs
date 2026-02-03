using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals
{
    /// <summary>
    /// Input for getting all rentals with optional status filter.
    /// </summary>
    public sealed class GetAllRentalsInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets the optional rental status filter.
        /// If null, returns all rentals regardless of status.
        /// </summary>
        public RentalStatus? Status { get; set; }
    }
}
