using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate
{
    /// <summary>
    /// Input for getting a rental by vehicle license plate.
    /// </summary>
    public sealed class GetRentalByLicensePlateInput : IUseCaseInput
    {
        /// <summary>
        /// Gets or sets the vehicle license plate.
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;
    }
}
