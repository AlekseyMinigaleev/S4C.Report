using C4S.Services.Implements;
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
            /*TODO: жизненные циклы зависимостей*/
            services.AddTransient<IBackGroundJobService, BackGroundJobService>();
            services.AddTransient<IGameIdSyncService, GameIdSyncService>();
            services.AddTransient<IGameDataService,GameDataService>();
            services.AddYandexGetewayServices(configuration);
        }
    }
}