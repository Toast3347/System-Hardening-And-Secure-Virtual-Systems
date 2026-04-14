using Microsoft.Extensions.Configuration;

namespace ComicRealmBE.Data.Configuration
{
    /// <summary>
    /// DatabaseConfig: Securely retrieves database connection details
    /// 
    /// Development Environment:
    /// - Reads from User Secrets (stored in Windows DPAPI encrypted storage)
    /// - Never hardcoded or committed to git
    /// 
    /// Production Environment (Docker):
    /// - Reads from environment variables
    /// - Injected by Docker container at runtime
    /// - Never exposed in code or docker-compose.yml
    /// 
    /// User Secrets Setup:
    /// dotnet user-secrets init
    /// dotnet user-secrets set "Database:Host" "localhost"
    /// dotnet user-secrets set "Database:Port" "5432"
    /// dotnet user-secrets set "Database:UserId" "postgres"
    /// dotnet user-secrets set "Database:Password" "YourSecurePassword123!"
    /// dotnet user-secrets set "Database:Name" "comicrealm_dev"
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Builds the PostgreSQL connection string from configuration
        /// </summary>
        public static string GetConnectionString(IConfiguration configuration)
        {
            var host = configuration["Database:Host"] ?? "localhost";
            var port = configuration["Database:Port"] ?? "5432";
            var userId = configuration["Database:UserId"] ?? "postgres";
            var password = configuration["Database:Password"];
            var database = configuration["Database:Name"] ?? "comicrealm_dev";

            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException(
                    "Database password not configured. " +
                    "For development: dotnet user-secrets set \"Database:Password\" \"YourPassword\" " +
                    "For production: Set DATABASE_PASSWORD environment variable");
            }

            // PostgreSQL connection string format
            // Host=localhost;Port=5432;Username=postgres;Password=secret;Database=comicrealm_dev;
            return $"Host={host};Port={port};Username={userId};Password={password};Database={database};";
        }

        /// <summary>
        /// Validates that all required database configuration is present
        /// </summary>
        public static void ValidateConfiguration(IConfiguration configuration)
        {
            var requiredKeys = new[] { "Database:Host", "Database:Port", "Database:UserId", "Database:Password", "Database:Name" };
            var missingKeys = requiredKeys.Where(key => string.IsNullOrEmpty(configuration[key])).ToList();

            if (missingKeys.Any())
            {
                throw new InvalidOperationException(
                    $"Missing required database configuration: {string.Join(", ", missingKeys)}. " +
                    $"Please set these values in User Secrets (development) or environment variables (production).");
            }
        }
    }
}
