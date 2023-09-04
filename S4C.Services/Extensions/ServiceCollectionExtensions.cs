using AngleSharp;
using C4S.Services.Implements;
using C4S.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using S4C.YandexGateway.DeveloperPage;
using S4C.YandexGateway.DeveloperPageGateway;

namespace C4S.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            /*TODO: жизненные циклы зависимостей*/
            services.AddTransient<IBackGroundJobService, BackGroundJobService>();
            services.AddTransient<IGameIdSyncService, GameIdSyncService>();
            services.AddTransient<IGameDataService,GameDataService>();
            services.AddTransient<IDeveloperPageGetaway,DeveloperPageGateway>();
            services.AddTransient <IDeveloperPageParser, DeveloperPageParser>();
            services.AddScoped((provider) =>
            {
                var config = Configuration.Default.WithDefaultLoader();
                var browsingContext = BrowsingContext.New(config);
                return browsingContext;
            });
        }
    }
}