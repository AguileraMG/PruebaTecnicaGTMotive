using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Input for creating a new vehicle in the fleet.
    /// </summary>
    public sealed class CreateVehicleInput : IUseCaseInput
    {
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
    }
}
