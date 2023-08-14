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

        public async Task AddOrUpdateRecurringJobAsync(HangfireJobConfigurationModel jobConfig)
        {
            var existence = await _dbContext.HangfireConfigurationModels
                .SingleAsync(x => x.JobType == jobConfig.JobType);

            existence.Update(jobConfig.CronExpression, jobConfig.IsEnable);

            AddOrUpdateRecurringJob(existence);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrUpdateAllRecurringJobAsync(HangfireJobConfigurationModel? jobConfig = null)
        {
            var existenceJobConfigurations = _dbContext.HangfireConfigurationModels;

            var jobTypes = Enum.GetValues(typeof(HangfireJobTypeEnum)).Cast<HangfireJobTypeEnum>();

            foreach (var jobType in jobTypes)
            {
                var existence = await existenceJobConfigurations
                    .SingleOrDefaultAsync(x => x.JobType == jobType);

                if (existence is null)
                {
                    existence = new HangfireJobConfigurationModel(
                       jobType: jobType,
                       cronExpression: HangfireJobConfigurationConstants.DefaultCronExpression,
                       isEnable: HangfireJobConfigurationConstants.DefaultIsEnable);

                    _dbContext.HangfireConfigurationModels.Add(existence);
                }

                if (jobConfig != null)
                    existence.Update(jobConfig.CronExpression, jobConfig.IsEnable!);

                AddOrUpdateRecurringJob(existence);
            }

            await _dbContext.SaveChangesAsync();
        }

        private static void AddOrUpdateRecurringJob(HangfireJobConfigurationModel jobConfig)
        {
            switch (jobConfig.JobType)
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
                    jobConfig.JobType.ToString(),
                    methodCall,
                    jobConfig.CronExpression ?? HangfireJobConfigurationConstants.DefaultCronExpression,  /*TODO: сделать проверку на валидность*/
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
    }
}