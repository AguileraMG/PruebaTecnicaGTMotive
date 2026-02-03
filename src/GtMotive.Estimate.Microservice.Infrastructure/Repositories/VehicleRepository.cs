#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// MongoDB implementation of the vehicle repository.
    /// </summary>
    public sealed class VehicleRepository : IVehicleRepository
    {
        private readonly IMongoCollection<Vehicle> _vehiclesCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleRepository"/> class.
        /// </summary>
        /// <param name="mongoService">The MongoDB service providing the client connection.</param>
        /// <param name="mongoDbSettings">The MongoDB settings containing database name.</param>
        public VehicleRepository(
            MongoService mongoService,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            ArgumentNullException.ThrowIfNull(mongoService);
            ArgumentNullException.ThrowIfNull(mongoDbSettings);

            // Get the database
            var database = mongoService.MongoClient.GetDatabase(
                mongoDbSettings.Value.DatabaseName);

            // Get the collection (will be created if it doesn't exist)
            _vehiclesCollection = database.GetCollection<Vehicle>("Vehicles");
        }

        public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            // Insert the vehicle into MongoDB
            await _vehiclesCollection.InsertOneAsync(vehicle, cancellationToken: cancellationToken);
        }

        public async Task<Vehicle?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            // Create filter: { _id: ObjectId(id) }
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);

            // Find and return the first match (or null)
            return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate);

            // Create filter: { licensePlate: "ABC123" }
            var filter = Builders<Vehicle>.Filter.Eq(v => v.LicensePlate, licensePlate);

            // Find and return the first match (or null)
            return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus? status, CancellationToken cancellationToken = default)
        {
            FilterDefinition<Vehicle> filter;

            if (status.HasValue)
            {
                // Filter by specific status
                filter = Builders<Vehicle>.Filter.Eq(v => v.Status, status.Value);
            }
            else
            {
                // Return all vehicles (empty filter)
                filter = Builders<Vehicle>.Filter.Empty;
            }

            return await _vehiclesCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            // Create filter: { _id: ObjectId(vehicle.Id) }
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, vehicle.Id);

            // Replace the entire document
            await _vehiclesCollection.ReplaceOneAsync(
                filter,
                vehicle,
                cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            // Create filter: { _id: ObjectId(id) }
            var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);

            // Delete the document
            await _vehiclesCollection.DeleteOneAsync(filter, cancellationToken);
        }
    }
}
