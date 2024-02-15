using C4S.DB;
using C4S.Helpers.Logger;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Выполняет все необходимые процессы, для корректной работы приложения
        /// </summary>
        /// <remarks>
        /// Подразумевает в момент выполнения наличие базы данных для Hangfire
        /// </remarks>
        public static async Task InitApplicationAsync(
            this WebApplication app,
            CancellationToken cancellationToken = default)
        {
            using var scope = app.Services.CreateScope();
            var (logger, dbContext) = GetDependencies(scope);

            logger.LogInformation("Начало выполнения миграций: ");
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Все миграции успешно выполнены");
        }

        private static (ConsoleLogger, ReportDbContext) GetDependencies(IServiceScope scope)
        {
            var services = scope.ServiceProvider;

            var defaultLogger = services.GetRequiredService<ILogger<Program>>();
            var logger = new ConsoleLogger(defaultLogger);

            var dbContext = services.GetRequiredService<ReportDbContext>();

            return (logger, dbContext);
        }
    }
}