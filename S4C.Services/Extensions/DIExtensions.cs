using C4S.Services.Implements;
using C4S.Services.Implements.Parsers;
using C4S.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace C4S.Services.Extensions
{
    public static class DIExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IBackGroundJobService, BackGroundJobService>();
            services.AddTransient<IParser, GameStatisticParser>();
        }
    }
}