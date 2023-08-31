using C4S.DB;
using C4S.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Инициализирует всю необходимую инфраструктуру приложения
        /// </summary>
        public static async Task InitApplicationInfrastructureAsync(
            this WebApplication app,
            CancellationToken cancellationToken = default)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ReportDbContext>();
            await context.Database.MigrateAsync(cancellationToken);

            var service = services.GetRequiredService<IBackGroundJobService>();
            await service.AddMissingHangfirejobsAsync(cancellationToken);
        }
    }
}