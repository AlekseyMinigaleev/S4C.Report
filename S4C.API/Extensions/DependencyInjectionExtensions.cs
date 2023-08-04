﻿using Hangfire;
using Microsoft.EntityFrameworkCore;
using S4C.DB;

namespace S4C.API.Extensions
{
    public static class DependencyInjectionExtensions
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