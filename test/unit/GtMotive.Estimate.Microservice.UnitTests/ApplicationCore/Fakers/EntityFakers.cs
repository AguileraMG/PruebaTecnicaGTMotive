using System;
using Bogus;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Fakers
{
    /// <summary>
    /// Provides Bogus faker instances for generating test data for domain entities.
    /// These fakers generate realistic, randomized data for use in unit tests.
    /// Uses factory methods to ensure domain invariants are satisfied.
    /// </summary>
    internal static class EntityFakers
    {
        /// <summary>
        /// Gets a Bogus faker for generating <see cref="Customer"/> test data.
        /// Uses Customer.Create() factory method to ensure all domain validations are applied.
        /// </summary>
        public static Faker<Customer> CustomerFaker => new Faker<Customer>()
            .CustomInstantiator(f => Customer.Create(
                name: f.Name.FullName(),
                email: f.Internet.Email(),
                phoneNumber: f.Phone.PhoneNumber("+34#########"),
                driverLicenseNumber: f.Random.AlphaNumeric(10).ToUpper(System.Globalization.CultureInfo.CurrentCulture)));

        /// <summary>
        /// Gets a Bogus faker for generating <see cref="Vehicle"/> test data.
        /// Uses Vehicle.Create() factory method to ensure all domain validations are applied.
        /// </summary>
        public static Faker<Vehicle> VehicleFaker => new Faker<Vehicle>()
            .CustomInstantiator(f => Vehicle.Create(
                brand: f.Vehicle.Manufacturer(),
                model: f.Vehicle.Model(),
                year: f.Date.Between(DateTime.UtcNow.AddYears(-5), DateTime.UtcNow).Year,
                licensePlate: f.Random.Replace("???-####"),
                kilometersDriven: f.Random.Int(0, 200000)));

        /// <summary>
        /// Gets a Bogus faker for generating <see cref="Rental"/> test data.
        /// Uses Rental.Create() factory method to ensure all domain validations are applied.
        /// </summary>
        public static Faker<Rental> RentalFaker => new Faker<Rental>()
            .CustomInstantiator(f => Rental.Create(
                vehicleId: Guid.NewGuid().ToString(),
                customerId: Guid.NewGuid().ToString(),
                expectedReturnDate: DateTime.UtcNow.AddDays(f.Random.Int(1, 30)),
                notes: f.Lorem.Sentence()));
    }
}
