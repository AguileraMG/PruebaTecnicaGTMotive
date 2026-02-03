using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    [Collection(TestCollections.Functional)]
    public abstract class FunctionalTestBase(CompositionRootTestFixture fixture) : IAsyncLifetime
    {
        public const int QueueWaitingTimeInMilliseconds = 1000;

        protected CompositionRootTestFixture Fixture { get; } = fixture;

        public async Task InitializeAsync()
        {
            // Limpiar la base de datos antes de cada test
            await CleanDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        private async Task CleanDatabaseAsync()
        {
            using var scope = Fixture.Configuration.GetSection("MongoDB:DatabaseName").Value != null ? Fixture.CreateScope()
                : null;

            if (scope != null)
            {
                var mongoService = scope.ServiceProvider.GetService<MongoService>();
                if (mongoService != null)
                {
                    var databaseName = Fixture.Configuration["MongoDB:DatabaseName"];
                    var database = mongoService.MongoClient.GetDatabase(databaseName);

                    await database.DropCollectionAsync("Vehicles");
                    await database.DropCollectionAsync("Customers");
                    await database.DropCollectionAsync("Rentals");
                }
            }
        }
    }

    public static class CompositionRootTestFixtureExtensions
    {
        public static IServiceScope CreateScope(this CompositionRootTestFixture fixture)
        {
            var serviceProvider = fixture.GetType()
                .GetField("_serviceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(fixture) as ServiceProvider;

            return serviceProvider?.CreateScope();
        }
    }
}
