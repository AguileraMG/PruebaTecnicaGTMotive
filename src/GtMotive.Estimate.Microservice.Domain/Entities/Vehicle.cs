using System;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents the status of a vehicle.
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        /// Vehicle is available for rent.
        /// </summary>
        Available = 0,

        /// <summary>
        /// Vehicle is currently rented.
        /// </summary>
        Rented = 1,

        /// <summary>
        /// Vehicle is retired from the fleet.
        /// </summary>
        Retired = 2
    }

    /// <summary>
    /// Represents a vehicle in the rental fleet.
    /// Domain entity with encapsulated business logic and invariants.
    /// </summary>
    public class Vehicle
    {
        private const int MaxVehicleAgeInYears = 5;

        /// <summary>
        /// Gets the unique identifier of the vehicle.
        /// </summary>
        public string Id { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the brand/manufacturer of the vehicle.
        /// </summary>
        public string Brand { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the model of the vehicle.
        /// </summary>
        public string Model { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the manufacturing year of the vehicle.
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Gets the license plate of the vehicle.
        /// </summary>
        public string LicensePlate { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the total kilometers driven by the vehicle.
        /// </summary>
        public int KilometersDriven { get; private set; }

        /// <summary>
        /// Gets the current status of the vehicle.
        /// </summary>
        public VehicleStatus Status { get; private set; }

        /// <summary>
        /// Gets the date and time when the vehicle was registered.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time of the last update.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Factory method to create a new vehicle with validation.
        /// </summary>
        /// <param name="brand">Vehicle brand/manufacturer.</param>
        /// <param name="model">Vehicle model.</param>
        /// <param name="year">Manufacturing year.</param>
        /// <param name="licensePlate">License plate number.</param>
        /// <param name="kilometersDriven">Initial kilometers driven.</param>
        /// <returns>A new valid Vehicle instance.</returns>
        /// <exception cref="DomainException">Thrown when validation fails.</exception>
        public static Vehicle Create(
            string brand,
            string model,
            int year,
            string licensePlate,
            int kilometersDriven)
        {
            ValidateBrand(brand);
            ValidateModel(model);
            ValidateYear(year);
            ValidateLicensePlate(licensePlate);
            ValidateKilometers(kilometersDriven);

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid().ToString(),
                Brand = brand,
                Model = model,
                Year = year,
                LicensePlate = licensePlate,
                KilometersDriven = kilometersDriven,
                Status = VehicleStatus.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            // Validate vehicle is eligible for fleet (not too old)
            return !vehicle.IsEligibleForFleet()
                ? throw new DomainException(
                    $"Vehicle is too old to be added to the fleet. Maximum age is {MaxVehicleAgeInYears} years. Vehicle year: {year}")
                : vehicle;
        }

        /// <summary>
        /// Checks if the vehicle is eligible to be part of the fleet.
        /// A vehicle is eligible if it's not older than the maximum allowed years.
        /// </summary>
        /// <returns>True if the vehicle is eligible, false otherwise.</returns>
        public bool IsEligibleForFleet()
        {
            var currentYear = DateTime.UtcNow.Year;
            var vehicleAge = currentYear - Year;
            return vehicleAge <= MaxVehicleAgeInYears;
        }

        /// <summary>
        /// Checks if the vehicle is currently available for rental.
        /// </summary>
        /// <returns>True if the vehicle is available, false otherwise.</returns>
        public bool IsAvailable()
        {
            return Status == VehicleStatus.Available;
        }

        /// <summary>
        /// Marks the vehicle as rented.
        /// </summary>
        /// <exception cref="DomainException">Thrown when vehicle is not available.</exception>
        public void MarkAsRented()
        {
            if (!IsAvailable())
            {
                throw new DomainException(
                    $"Vehicle '{LicensePlate}' cannot be rented. Current status: {Status}");
            }

            Status = VehicleStatus.Rented;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the vehicle as available for rental.
        /// </summary>
        public void MarkAsAvailable()
        {
            if (Status == VehicleStatus.Available)
            {
                throw new DomainException($"Vehicle '{LicensePlate}' is already available.");
            }

            Status = VehicleStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the vehicle as retired from the fleet.
        /// </summary>
        public void MarkAsRetired()
        {
            if (Status == VehicleStatus.Retired)
            {
                throw new DomainException($"Vehicle '{LicensePlate}' is already retired.");
            }

            Status = VehicleStatus.Retired;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds kilometers to the vehicle's odometer.
        /// </summary>
        /// <param name="kilometers">Kilometers to add (must be positive).</param>
        /// <exception cref="DomainException">Thrown when kilometers is negative or zero.</exception>
        public void AddKilometers(int kilometers)
        {
            if (kilometers <= 0)
            {
                throw new DomainException("Kilometers to add must be greater than zero.");
            }

            KilometersDriven += kilometers;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets the vehicle's odometer reading.
        /// Cannot be less than the current reading (odometer cannot go backwards).
        /// </summary>
        /// <param name="newKilometers">New odometer reading.</param>
        /// <exception cref="DomainException">Thrown when new reading is less than current.</exception>
        public void SetKilometers(int newKilometers)
        {
            if (newKilometers < 0)
            {
                throw new DomainException("Kilometers cannot be negative.");
            }

            if (newKilometers < KilometersDriven)
            {
                throw new DomainException(
                    $"Invalid odometer reading. Vehicle currently has {KilometersDriven} km, cannot be set to {newKilometers} km.");
            }

            KilometersDriven = newKilometers;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
            {
                throw new DomainException("Vehicle brand cannot be empty.");
            }

            if (brand.Length < 2)
            {
                throw new DomainException("Vehicle brand must be at least 2 characters long.");
            }

            if (brand.Length > 50)
            {
                throw new DomainException("Vehicle brand cannot exceed 50 characters.");
            }
        }

        private static void ValidateModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                throw new DomainException("Vehicle model cannot be empty.");
            }

            if (model.Length < 1)
            {
                throw new DomainException("Vehicle model must be at least 1 character long.");
            }

            if (model.Length > 50)
            {
                throw new DomainException("Vehicle model cannot exceed 50 characters.");
            }
        }

        private static void ValidateYear(int year)
        {
            var currentYear = DateTime.UtcNow.Year;
            var minYear = 1900;

            if (year < minYear)
            {
                throw new DomainException($"Vehicle year cannot be earlier than {minYear}.");
            }

            if (year > currentYear + 1)
            {
                throw new DomainException($"Vehicle year cannot be in the future (current year: {currentYear}).");
            }
        }

        private static void ValidateLicensePlate(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                throw new DomainException("License plate cannot be empty.");
            }

            if (licensePlate.Length < 4)
            {
                throw new DomainException("License plate must be at least 4 characters long.");
            }

            if (licensePlate.Length > 20)
            {
                throw new DomainException("License plate cannot exceed 20 characters.");
            }
        }

        private static void ValidateKilometers(int kilometers)
        {
            if (kilometers < 0)
            {
                throw new DomainException("Kilometers cannot be negative.");
            }

            if (kilometers > 1000000)
            {
                throw new DomainException("Kilometers cannot exceed 1,000,000.");
            }
        }
    }
}
