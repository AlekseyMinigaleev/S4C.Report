using C4S.DB;
using C4S.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task InitApplicationAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ReportDbContext>();
                await context.Database.MigrateAsync();

                var service = services.GetRequiredService<IBackGroundJobService>();
                await service.AddMissingHangfirejobs();
            }
        }
    }
}