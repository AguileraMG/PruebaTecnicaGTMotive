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
    /// MongoDB implementation of the rental repository.
    /// </summary>
    public sealed class RentalRepository : IRentalRepository
    {
        private readonly IMongoCollection<Rental> _rentalsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRepository"/> class.
        /// </summary>
        /// <param name="mongoService">The MongoDB service providing the client connection.</param>
        /// <param name="mongoDbSettings">The MongoDB settings containing database name.</param>
        public RentalRepository(
            MongoService mongoService,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            ArgumentNullException.ThrowIfNull(mongoService);
            ArgumentNullException.ThrowIfNull(mongoDbSettings);

            var database = mongoService.MongoClient.GetDatabase(
                mongoDbSettings.Value.DatabaseName);

            _rentalsCollection = database.GetCollection<Rental>("Rentals");
        }

        /// <inheritdoc/>
        public async Task AddAsync(Rental rental, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rental);

            await _rentalsCollection.InsertOneAsync(rental, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Rental?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            var filter = Builders<Rental>.Filter.Eq(r => r.Id, id);
            return await _rentalsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Rental>> GetActiveRentalsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

            var filter = Builders<Rental>.Filter.And(
                Builders<Rental>.Filter.Eq(r => r.CustomerId, customerId),
                Builders<Rental>.Filter.Eq(r => r.Status, RentalStatus.Active));

            return await _rentalsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Rental>> GetRentalsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

            var filter = Builders<Rental>.Filter.Eq(r => r.CustomerId, customerId);
            return await _rentalsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Rental?> GetActiveRentalByVehicleIdAsync(string vehicleId, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(vehicleId);

            var filter = Builders<Rental>.Filter.And(
                Builders<Rental>.Filter.Eq(r => r.VehicleId, vehicleId),
                Builders<Rental>.Filter.Eq(r => r.Status, RentalStatus.Active));

            return await _rentalsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken = default)
        {
            var filter = Builders<Rental>.Filter.Eq(r => r.Status, RentalStatus.Active);
            return await _rentalsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Rental>> GetOverdueRentalsAsync(CancellationToken cancellationToken = default)
        {
            var filter = Builders<Rental>.Filter.And(
                Builders<Rental>.Filter.Eq(r => r.Status, RentalStatus.Active),
                Builders<Rental>.Filter.Lt(r => r.ExpectedReturnDate, DateTime.UtcNow));

            return await _rentalsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Rental>> GetAllRentalsAsync(RentalStatus? status = null, CancellationToken cancellationToken = default)
        {
            var filter = status.HasValue ? Builders<Rental>.Filter.Eq(r => r.Status, status.Value) : Builders<Rental>.Filter.Empty;
            return await _rentalsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rental);

            var filter = Builders<Rental>.Filter.Eq(r => r.Id, rental.Id);
            await _rentalsCollection.ReplaceOneAsync(
                filter,
                rental,
                cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            var filter = Builders<Rental>.Filter.Eq(r => r.Id, id);
            await _rentalsCollection.DeleteOneAsync(filter, cancellationToken);
        }
    }
}
