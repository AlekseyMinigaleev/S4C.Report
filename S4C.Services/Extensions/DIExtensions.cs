using Microsoft.Extensions.DependencyInjection;
using C4S.Services.Implements;
using C4S.Services.Interfaces;

namespace C4S.Services.Extensions
{
    public static class DIExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IBackGroundJobService, BackGroundJobService>();
        }
    }
}
