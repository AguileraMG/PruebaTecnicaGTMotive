namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings
{
    /// <summary>
    /// MongoDB connection settings.
    /// </summary>
    public class MongoDbSettings
    {
        /// <summary>
        /// Gets or sets the MongoDB connection string.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MongoDB database name.
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MongoDB database name (legacy property for backward compatibility).
        /// </summary>
        public string MongoDbDatabaseName
        {
            get => DatabaseName;
            set => DatabaseName = value;
        }
    }
}
