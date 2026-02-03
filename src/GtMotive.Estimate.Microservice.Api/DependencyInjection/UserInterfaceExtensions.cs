using GtMotive.Estimate.Microservice.Api.UseCases.Customers;
using GtMotive.Estimate.Microservice.Api.UseCases.Rentals;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Api.DependencyInjection
{
    /// <summary>
    /// Extension methods for registering API presenters.
    /// </summary>
    public static class UserInterfaceExtensions
    {
        /// <summary>
        /// Registers all presenters in the dependency injection container.
        /// Each presenter is registered both as itself and as its output port interface.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            // Vehicle presenters
            services.AddScoped<CreateVehiclePresenter>();
            services.AddScoped<ICreateVehicleOutputPort>(sp =>
                sp.GetRequiredService<CreateVehiclePresenter>());

            services.AddScoped<GetVehiclesByStatusPresenter>();
            services.AddScoped<IGetVehiclesByStatusOutputPort>(sp =>
                sp.GetRequiredService<GetVehiclesByStatusPresenter>());

            // Customer presenters
            services.AddScoped<CreateCustomerPresenter>();
            services.AddScoped<ICreateCustomerOutputPort>(sp =>
                sp.GetRequiredService<CreateCustomerPresenter>());

            services.AddScoped<GetAllCustomersPresenter>();
            services.AddScoped<IGetAllCustomersOutputPort>(sp =>
                sp.GetRequiredService<GetAllCustomersPresenter>());

            // Rental presenters
            services.AddScoped<RentVehiclePresenter>();
            services.AddScoped<IRentVehicleOutputPort>(sp =>
                sp.GetRequiredService<RentVehiclePresenter>());

            services.AddScoped<ReturnVehiclePresenter>();
            services.AddScoped<IReturnVehicleOutputPort>(sp =>
                sp.GetRequiredService<ReturnVehiclePresenter>());

            services.AddScoped<GetRentalByLicensePlatePresenter>();
            services.AddScoped<IGetRentalByLicensePlateOutputPort>(sp =>
                sp.GetRequiredService<GetRentalByLicensePlatePresenter>());

            services.AddScoped<GetAllRentalsPresenter>();
            services.AddScoped<IGetAllRentalsOutputPort>(sp =>
                sp.GetRequiredService<GetAllRentalsPresenter>());

            return services;
        }
    }
}
