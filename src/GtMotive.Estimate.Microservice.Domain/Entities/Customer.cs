using System;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents a customer in the system.
    /// Domain entity with encapsulated business logic and invariants.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets the unique identifier of the customer.
        /// </summary>
        public string Id { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the full name of the customer.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the email address of the customer.
        /// </summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the phone number of the customer.
        /// </summary>
        public string PhoneNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the driver's license number of the customer.
        /// </summary>
        public string DriverLicenseNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the customer currently has an active rental.
        /// </summary>
        public bool HasActiveRental { get; private set; }

        /// <summary>
        /// Gets the date and time when the customer was registered.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time of the last update.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Factory method to create a new customer with validation.
        /// </summary>
        /// <param name="name">Customer full name.</param>
        /// <param name="email">Customer email address.</param>
        /// <param name="phoneNumber">Customer phone number.</param>
        /// <param name="driverLicenseNumber">Customer driver license number.</param>
        /// <returns>A new valid Customer instance.</returns>
        /// <exception cref="DomainException">Thrown when validation fails.</exception>
        public static Customer Create(
            string name,
            string email,
            string phoneNumber,
            string driverLicenseNumber)
        {
            ValidateName(name);
            ValidateEmail(email);
            ValidatePhoneNumber(phoneNumber);
            ValidateDriverLicense(driverLicenseNumber);

            return new Customer
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                DriverLicenseNumber = driverLicenseNumber,
                HasActiveRental = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        /// <summary>
        /// Validates if the customer can rent a vehicle.
        /// A customer cannot rent more than one vehicle at the same time.
        /// </summary>
        /// <returns>True if the customer can rent a vehicle.</returns>
        public bool CanRentVehicle()
        {
            return !HasActiveRental;
        }

        /// <summary>
        /// Marks the customer as having an active rental.
        /// </summary>
        /// <exception cref="DomainException">Thrown when customer already has an active rental.</exception>
        public void MarkAsRenting()
        {
            if (HasActiveRental)
            {
                throw new DomainException($"Customer {Name} already has an active rental.");
            }

            HasActiveRental = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the customer as not having an active rental.
        /// </summary>
        /// <exception cref="DomainException">Thrown when customer doesn't have an active rental.</exception>
        public void MarkAsNotRenting()
        {
            if (!HasActiveRental)
            {
                throw new DomainException($"Customer {Name} does not have an active rental.");
            }

            HasActiveRental = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the customer's contact information.
        /// </summary>
        /// <param name="phoneNumber">New phone number.</param>
        /// <exception cref="DomainException">Thrown when phone number is invalid.</exception>
        public void UpdatePhoneNumber(string phoneNumber)
        {
            ValidatePhoneNumber(phoneNumber);
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the customer's name.
        /// </summary>
        /// <param name="name">New name.</param>
        /// <exception cref="DomainException">Thrown when name is invalid.</exception>
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Customer name cannot be empty.");
            }

            if (name.Length < 2)
            {
                throw new DomainException("Customer name must be at least 2 characters long.");
            }

            if (name.Length > 200)
            {
                throw new DomainException("Customer name cannot exceed 200 characters.");
            }
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException("Customer email cannot be empty.");
            }

            if (!email.Contains('@', StringComparison.Ordinal) || !email.Contains('.', StringComparison.Ordinal))
            {
                throw new DomainException("Customer email format is invalid.");
            }

            if (email.Length > 255)
            {
                throw new DomainException("Customer email cannot exceed 255 characters.");
            }
        }

        private static void ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new DomainException("Customer phone number cannot be empty.");
            }

            if (phoneNumber.Length < 9)
            {
                throw new DomainException("Customer phone number must be at least 9 characters long.");
            }

            if (phoneNumber.Length > 20)
            {
                throw new DomainException("Customer phone number cannot exceed 20 characters.");
            }
        }

        private static void ValidateDriverLicense(string driverLicenseNumber)
        {
            if (string.IsNullOrWhiteSpace(driverLicenseNumber))
            {
                throw new DomainException("Driver license number cannot be empty.");
            }

            if (driverLicenseNumber.Length < 5)
            {
                throw new DomainException("Driver license number must be at least 5 characters long.");
            }

            if (driverLicenseNumber.Length > 50)
            {
                throw new DomainException("Driver license number cannot exceed 50 characters.");
            }
        }
    }
}
