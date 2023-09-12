using C4S.Services.Implements;
using C4S.Services.Implements.ReportExcelFile;
using C4S.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using S4C.YandexGateway.DeveloperPage.Extensions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace C4S.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IHangfireBackgroundJobService, BackgroundJobService>();
            services.AddScoped<IGameIdSyncService, GameIdSyncService>();
            services.AddScoped<IGameDataService, GameDataService>();
            services.AddScoped<IReportExcelFileService, ReportExcelFileService>();
            services.AddYandexGetewayServices(configuration);
        }
    }
}