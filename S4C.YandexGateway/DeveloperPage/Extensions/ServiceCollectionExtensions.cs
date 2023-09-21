using AngleSharp;
using Microsoft.Extensions.DependencyInjection;
using S4C.YandexGateway.RSYA;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace S4C.YandexGateway.DeveloperPage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddYandexGetewayServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddScoped<IDeveloperPageGetaway, DeveloperPageGateway>();
            services.AddScoped<IDeveloperPageParser, DeveloperPageParser>();
            services.AddScoped((provider) =>
            {
                var config = Configuration.Default.WithDefaultLoader();
                var browsingContext = BrowsingContext.New(config);
                return browsingContext;
            });

            services.AddTransient<IRsyaGateway, RsyaGateway>();
        }
    }
}