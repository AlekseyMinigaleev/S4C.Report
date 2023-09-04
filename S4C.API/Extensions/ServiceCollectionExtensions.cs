using C4S.DB;
using Hangfire;
using Hangfire.Console;
using Microsoft.EntityFrameworkCore;

namespace C4S.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет все необходимые сервисы для хранилищ
        /// </summary>
        public static void AddStorages(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("ReportDbDev");

            services.AddDbContext<ReportDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHangfire(configuration => configuration
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(connectionString)
                   .UseConsole());

            services.AddHangfireServer();
        }
    }
}