using C4S.DB;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
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
            var (logger, backGroundJobService, dbContext) = GetDependencies(scope);

            logger.LogInformation("Начало выполнения миграций: ");
            dbContext.Database.Migrate();
            logger.LogInformation("Все миграции успешно выполнены");

            await backGroundJobService.AddMissingHangfirejobsAsync(logger, cancellationToken);
        }

        private static (ConsoleLogger<Program>, IBackGroundJobService, ReportDbContext) GetDependencies(IServiceScope scope)
        {
            var services = scope.ServiceProvider;

            var defaultLogger = services.GetRequiredService<ILogger<Program>>();
            var logger = new ConsoleLogger<Program>(defaultLogger);
            var backGroundJobService = services.GetRequiredService<IBackGroundJobService>();
            var dbContext = services.GetRequiredService<ReportDbContext>();

            return (logger, backGroundJobService, dbContext);
        }
    }
}