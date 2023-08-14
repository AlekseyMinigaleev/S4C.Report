using Hangfire;
using Microsoft.EntityFrameworkCore;
using C4S.DB;
using C4S.Services.Implements;
using C4S.Services.Interfaces;

namespace C4S.API.Extensions
{
    public static class DIExtensions
    {
        public static void AddStorage(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("ReportDbDev");

            services.AddDbContext<ReportDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHangfire(configuration => configuration
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(connectionString));

            services.AddHangfireServer();
        }
    }
}