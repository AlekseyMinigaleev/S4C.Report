using C4S.Common.ConfigurationModels;
using C4S.Services.Implements;
using C4S.Services.Implements.ExcelFileServices;
using C4S.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddScoped<IExcelWorksheetService, DetailedReportService>();
            services.AddScoped<ICategoriesSyncService,CategoriesSyncService>();
            services.AddScoped<IJwtService, JwtServise>((provider) =>
            {
                var jwtConfig = provider.GetService<IOptions<JwtConfiguration>>();

                if (jwtConfig?.Value is null)
                    throw new ArgumentNullException(nameof(IOptions<JwtConfiguration>));

                var service = new JwtServise(jwtConfig.Value);

                return service;
            });
            services.AddYandexGetewayServices(configuration);
        }
    }
}