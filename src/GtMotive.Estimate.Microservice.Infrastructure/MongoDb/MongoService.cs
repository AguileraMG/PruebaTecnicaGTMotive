using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// MongoDB service for managing database connections and entity mappings.
    /// </summary>
    public class MongoService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoService"/> class.
        /// </summary>
        /// <param name="options">MongoDB configuration settings.</param>
        public MongoService(IOptions<MongoDbSettings> options)
        {
            MongoClient = new MongoClient(options.Value.ConnectionString);

            // Register entity mappings and conventions
            RegisterBsonClasses();
        }

        /// <summary>
        /// Gets the MongoDB client instance.
        /// </summary>
        public MongoClient MongoClient { get; }

        /// <summary>
        /// Registers BSON class mappings and conventions for domain entities.
        /// This configures how C# classes are serialized/deserialized to/from MongoDB.
        /// </summary>
        private static void RegisterBsonClasses()
        {
            // 1. Register conventions (naming, enum handling, etc.)
            RegisterConventions();

            // 2. Register entity class mappings
            RegisterVehicleMapping();
            RegisterCustomerMapping();
            RegisterRentalMapping();
        }

        /// <summary>
        /// Registers global MongoDB conventions.
        /// </summary>
        private static void RegisterConventions()
        {
            var conventionPack = new ConventionPack
            {
                // Convert C# property names to camelCase in MongoDB
                // Example: "LicensePlate" -> "licensePlate"
                new CamelCaseElementNameConvention(),

                // Ignore extra fields in MongoDB that don't exist in C# class
                new IgnoreExtraElementsConvention(true),

                // Store enums as strings instead of integers
                // Example: VehicleStatus.Available -> "Available" (not 0)
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("FleetManagementConventions", conventionPack, t => true);
        }

        /// <summary>
        /// Registers the Vehicle entity mapping.
        /// </summary>
        private static void RegisterVehicleMapping()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Vehicle)))
            {
                BsonClassMap.RegisterClassMap<Vehicle>(cm =>
                {
                    // Auto-map all properties
                    cm.AutoMap();

                    // Configure the Id property
                    cm.MapIdProperty(v => v.Id)
     .SetSerializer(new StringSerializer(BsonType.String));

                    // Ignore extra elements
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        /// <summary>
        /// Registers the Customer entity mapping.
        /// </summary>
        private static void RegisterCustomerMapping()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Customer)))
            {
                BsonClassMap.RegisterClassMap<Customer>(cm =>
                {
                    cm.AutoMap();

                    cm.MapIdProperty(v => v.Id)
    .SetSerializer(new StringSerializer(BsonType.String));

                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        /// <summary>
        /// Registers the Rental entity mapping.
        /// </summary>
        private static void RegisterRentalMapping()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Rental)))
            {
                BsonClassMap.RegisterClassMap<Rental>(cm =>
                {
                    cm.AutoMap();

                    cm.MapIdProperty(v => v.Id)
    .SetSerializer(new StringSerializer(BsonType.String));

                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
