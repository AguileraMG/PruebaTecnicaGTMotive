using System;
using System.Diagnostics.CodeAnalysis;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.ApplicationCore
{
    /// <summary>
    /// Adds Use Cases classes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        /// <summary>
        /// Adds Use Cases to the ServiceCollection.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <returns>The modified instance.</returns>
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            // Vehicle use cases
            services.AddScoped<IUseCase<CreateVehicleInput>, CreateVehicleUseCase>();
            services.AddScoped<IUseCase<GetVehiclesByStatusInput>, GetVehiclesByStatusUseCase>();

            // Customer use cases
            services.AddScoped<IUseCase<CreateCustomerInput>, CreateCustomerUseCase>();
            services.AddScoped<IUseCase<GetAllCustomersInput>, GetAllCustomersUseCase>();

            // Rental use cases
            services.AddScoped<IUseCase<RentVehicleInput>, RentVehicleUseCase>();
            services.AddScoped<IUseCase<ReturnVehicleInput>, ReturnVehicleUseCase>();
            services.AddScoped<IUseCase<GetRentalByLicensePlateInput>, GetRentalByLicensePlateUseCase>();
            services.AddScoped<IUseCase<GetAllRentalsInput>, GetAllRentalsUseCase>();

            return services;
        }
    }
}
