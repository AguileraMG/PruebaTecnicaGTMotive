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
    /// MongoDB implementation of the customer repository.
    /// </summary>
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customersCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="mongoService">The MongoDB service providing the client connection.</param>
        /// <param name="mongoDbSettings">The MongoDB settings containing database name.</param>
        public CustomerRepository(
            MongoService mongoService,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            ArgumentNullException.ThrowIfNull(mongoService);
            ArgumentNullException.ThrowIfNull(mongoDbSettings);

            var database = mongoService.MongoClient.GetDatabase(
                mongoDbSettings.Value.DatabaseName);

            _customersCollection = database.GetCollection<Customer>("Customers");
        }

        /// <inheritdoc/>
        public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(customer);

            await _customersCollection.InsertOneAsync(customer, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
            return await _customersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            var filter = Builders<Customer>.Filter.Eq(c => c.Email, email);
            return await _customersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Customer?> GetByDriverLicenseAsync(string driverLicenseNumber, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(driverLicenseNumber);

            var filter = Builders<Customer>.Filter.Eq(c => c.DriverLicenseNumber, driverLicenseNumber);
            return await _customersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Customer>> GetAllAsync(bool? hasActiveRental = null, CancellationToken cancellationToken = default)
        {
            var filter = hasActiveRental.HasValue
                ? Builders<Customer>.Filter.Eq(c => c.HasActiveRental, hasActiveRental.Value)
                : Builders<Customer>.Filter.Empty;
            return await _customersCollection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(customer);

            var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
            await _customersCollection.ReplaceOneAsync(
                filter,
                customer,
                cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
            await _customersCollection.DeleteOneAsync(filter, cancellationToken);
        }
    }
}
