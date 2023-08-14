using Hangfire;
using Microsoft.EntityFrameworkCore;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Services.Interfaces;
using System.Linq.Expressions;

namespace C4S.Services.Implements
{
    public class BackGroundJobService : IBackGroundJobService
    {
        private readonly ReportDbContext _dbContext;

        public BackGroundJobService(ReportDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddOrUpdateRecurringJobAsync(HangfireJobConfigurationModel? jobConfig)
        {
            var existencesJobConfigurations = _dbContext.HangfireConfigurationModels;

            var jobTypes = Enum.GetValues(typeof(HangfireJobTypeEnum)).Cast<HangfireJobTypeEnum>();

            foreach (var jobType in jobTypes)
            {
                var existence = await existencesJobConfigurations
                    .SingleOrDefaultAsync(x => x.JopType == jobType);

                if (existence is null)
                {
                    existence = new HangfireJobConfigurationModel(
                       jobType: jobType,
                       cronExpression: HangfireJobConfigurationConstants.DefaultCronExpression,
                       isEnable: HangfireJobConfigurationConstants.DefaultIsEnable);

                    _dbContext.HangfireConfigurationModels.Add(existence);
                }

                if (jobConfig != null)
                    existence.Update(jobConfig!);

                AddOrUpdateRecurringJob(existence);
            }

            await _dbContext.SaveChangesAsync();
        }

        private static void AddOrUpdateRecurringJob(HangfireJobConfigurationModel jobConfig)
        {
            switch (jobConfig.JopType)
            {
                case HangfireJobTypeEnum.ParseGameStatisticFromDeveloperPage:
                    AddOrUpdateRecurringJob<DeveloperPageParser>(
                        jobConfig,
                        (service) => service.ParseAsync());
                    break;
                default:
                    break;
            }
        }

        private static void AddOrUpdateRecurringJob<T>(
            HangfireJobConfigurationModel jobConfig,
            Expression<Func<T, Task>> methodCall) =>
                RecurringJob.AddOrUpdate(
                    jobConfig.JopType.ToString(),
                    methodCall,
                    jobConfig.CronExpression ?? "* * * * *",  /*TODO: сделать проверку на валидность*/
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
    }
}