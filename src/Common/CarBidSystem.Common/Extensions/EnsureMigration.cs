using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CarBidSystem.Common.Extensions
{
    public static class EnsureMigration
    {
        public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app) where T : DbContext
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<T>();

                int maxRetries = 5;
                int retryDelay = 2000; // 2 seconds

                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        if (!dbContext.Database.CanConnect())
                        {
                            Console.WriteLine($"Database for {typeof(T).Name} is not ready. Retrying ({i + 1}/{maxRetries})...");
                            Thread.Sleep(retryDelay);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error connecting to the database: {ex.Message}");
                        if (i == maxRetries - 1) throw; // Rethrow on final retry
                    }
                }

                // Apply migrations
                dbContext.Database.Migrate();
                Console.WriteLine($"Migrations applied for {typeof(T).Name}.");
            }
        }
    }
}
