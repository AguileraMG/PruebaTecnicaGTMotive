using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.Infrastructure.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace GtMotive.Estimate.Microservice.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring MongoDB services.
    /// </summary>
    public static class MongoDbExtensions
    {
        /// <summary>
        /// Adds MongoDB services and repositories to the service collection.
        /// </summary>
        /// <param name="builder">The infrastructure builder.</param>
        /// <param name="configuration">The configuration containing MongoDB settings.</param>
        /// <returns>The infrastructure builder for chaining.</returns>
        public static IInfrastructureBuilder AddMongoDb(
            this IInfrastructureBuilder builder,
            IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configuration);

            // Bind MongoDB settings from configuration and register as IOptions
            builder.Services.AddSingleton(sp =>
            {
                var mongoDbSection = configuration.GetSection("MongoDb");
                var mongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = mongoDbSection["ConnectionString"],
                    DatabaseName = mongoDbSection["DatabaseName"]
                };
                return Options.Create(mongoDbSettings);
            });

            // Register MongoService as Singleton (MongoDB client is thread-safe and should be reused)
            builder.Services.AddSingleton<MongoService>();

            // Register repositories as Scoped (one instance per HTTP request)
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IRentalRepository, RentalRepository>();

            return builder;
        }
    }
}
