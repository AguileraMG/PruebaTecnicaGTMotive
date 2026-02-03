using System;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents the status of a rental.
    /// </summary>
    public enum RentalStatus
    {
        /// <summary>
        /// Rental is currently active.
        /// </summary>
        Active = 0,

        /// <summary>
        /// Rental has been completed.
        /// </summary>
        Completed = 1
    }

    /// <summary>
    /// Represents a vehicle rental transaction.
    /// Domain entity with encapsulated business logic and invariants.
    /// </summary>
    public class Rental
    {
        /// <summary>
        /// Gets the unique identifier of the rental.
        /// </summary>
        public string Id { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the identifier of the vehicle being rented.
        /// </summary>
        public string VehicleId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the identifier of the customer renting the vehicle.
        /// </summary>
        public string CustomerId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the date and time when the rental started.
        /// </summary>
        public DateTime RentalDate { get; private set; }

        /// <summary>
        /// Gets the date and time when the vehicle was returned.
        /// </summary>
        public DateTime? ReturnDate { get; private set; }

        /// <summary>
        /// Gets the expected return date.
        /// </summary>
        public DateTime ExpectedReturnDate { get; private set; }

        /// <summary>
        /// Gets the current status of the rental.
        /// </summary>
        public RentalStatus Status { get; private set; }

        /// <summary>
        /// Gets additional notes or comments about the rental.
        /// </summary>
        public string Notes { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the date and time when the rental record was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time of the last update.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Gets the duration of the rental in days.
        /// </summary>
        /// <returns>The number of days the vehicle was rented, or null if not yet returned.</returns>
        public int? RentalDurationInDays => ReturnDate.HasValue
            ? (ReturnDate.Value - RentalDate).Days
            : null;

        /// <summary>
        /// Factory method to create a new rental with validation.
        /// </summary>
        /// <param name="vehicleId">ID of the vehicle to rent.</param>
        /// <param name="customerId">ID of the customer renting.</param>
        /// <param name="expectedReturnDate">Expected return date.</param>
        /// <param name="notes">Optional notes.</param>
        /// <returns>A new valid Rental instance.</returns>
        /// <exception cref="DomainException">Thrown when validation fails.</exception>
        public static Rental Create(
            string vehicleId,
            string customerId,
            DateTime expectedReturnDate,
            string notes = null)
        {
            ValidateVehicleId(vehicleId);
            ValidateCustomerId(customerId);
            ValidateExpectedReturnDate(expectedReturnDate);

            return new Rental
            {
                Id = Guid.NewGuid().ToString(),
                VehicleId = vehicleId,
                CustomerId = customerId,
                RentalDate = DateTime.UtcNow,
                ExpectedReturnDate = expectedReturnDate,
                ReturnDate = null,
                Status = RentalStatus.Active,
                Notes = notes ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        /// <summary>
        /// Checks if the rental is currently active.
        /// </summary>
        /// <returns>True if the rental status is Active.</returns>
        public bool IsActive()
        {
            return Status == RentalStatus.Active;
        }

        /// <summary>
        /// Checks if the rental is overdue based on the expected return date.
        /// </summary>
        /// <returns>True if the rental is active and past the expected return date.</returns>
        public bool IsOverdue()
        {
            return IsActive() && DateTime.UtcNow > ExpectedReturnDate;
        }

        /// <summary>
        /// Completes the rental by setting the return date and marking it as completed.
        /// </summary>
        /// <exception cref="DomainException">Thrown when rental is not active.</exception>
        public void CompleteRental()
        {
            if (!IsActive())
            {
                throw new DomainException($"Rental {Id} is not active and cannot be completed. Current status: {Status}");
            }

            ReturnDate = DateTime.UtcNow;
            Status = RentalStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds or appends a note to the rental.
        /// </summary>
        /// <param name="note">The note or comment to add to the rental.</param>
        /// <exception cref="DomainException">Thrown when note is null or whitespace.</exception>
        public void AddNote(string note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return; // Silently ignore empty notes
            }

            if (note.Length > 500)
            {
                throw new DomainException("Note cannot exceed 500 characters.");
            }

            Notes = string.IsNullOrWhiteSpace(Notes)
                ? note
                : $"{Notes}\n{note}";

            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the expected return date.
        /// Can only be done while rental is active.
        /// </summary>
        /// <param name="newExpectedReturnDate">New expected return date.</param>
        /// <exception cref="DomainException">Thrown when rental is not active or date is invalid.</exception>
        public void UpdateExpectedReturnDate(DateTime newExpectedReturnDate)
        {
            if (!IsActive())
            {
                throw new DomainException($"Cannot update expected return date for rental {Id}. Current status: {Status}");
            }

            ValidateExpectedReturnDate(newExpectedReturnDate);

            ExpectedReturnDate = newExpectedReturnDate;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateVehicleId(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                throw new DomainException("Vehicle ID cannot be empty.");
            }
        }

        private static void ValidateCustomerId(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new DomainException("Customer ID cannot be empty.");
            }
        }

        private static void ValidateExpectedReturnDate(DateTime expectedReturnDate)
        {
            if (expectedReturnDate <= DateTime.UtcNow)
            {
                throw new DomainException("Expected return date must be in the future.");
            }

            var maxRentalPeriod = DateTime.UtcNow.AddDays(90); // 3 months max
            if (expectedReturnDate > maxRentalPeriod)
            {
                throw new DomainException("Expected return date cannot exceed 90 days from now.");
            }
        }
    }
}
