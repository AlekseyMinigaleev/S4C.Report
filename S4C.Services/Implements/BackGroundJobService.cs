using Hangfire;
using S4C.DB;
using S4C.DB.Models.Hangfire;
using S4C.Services.Interfaces;
using System.Linq.Expressions;

namespace S4C.Services.Implements
{
    public class BackGroundJobService : IBackGroundJobService
    {
        private readonly ReportDbContext _dbContext;
        private const string? defaultCronExpression = null; /*TODO: уточнить*/
        private const bool defaultIsEnable = true; /*TODO: уточнить*/

        public BackGroundJobService(ReportDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitJobsAsync()
        {
            var jobConfigurations = _dbContext.HangfireConfigurationModels;

            var jobTypes = Enum.GetValues(typeof(HangfireJobTypeEnum)).Cast<HangfireJobTypeEnum>();

            var missingJobs = new List<HangfireJobConfigurationModel>();

            foreach (var jobType in jobTypes)
            {
                HangfireJobConfigurationModel jobConfig;

                if (!jobConfigurations.Any(x => x.JopType == jobType))
                {
                    jobConfig = new HangfireJobConfigurationModel(
                        jobType: jobType,
                        cronExpression: defaultCronExpression,
                        isEnable:defaultIsEnable);
                }
                else
                {
                    jobConfig = jobConfigurations
                        .Single(x => x.JopType == jobType);
                }

                _dbContext.HangfireConfigurationModels.Add(jobConfig);

                /*TODO: пока коментим, тк нет интерфесов джоб*/
                //AddOrUpdateRecurringJob(jobConfig);
            }

            await _dbContext.SaveChangesAsync();
        }

        private void AddOrUpdateRecurringJob(HangfireJobConfigurationModel jobConfig)
        {
            switch (jobConfig.JopType)
            {
                case HangfireJobTypeEnum.ParseGameStatisticFromDeveloperPage:
                    AddOrUpdateRecurringJob<IParser>(
                        jobConfig,
                        (service) => service.Parse());
                    break;
                case HangfireJobTypeEnum.ParseGameStatisticFromDENGI:
                    AddOrUpdateRecurringJob<IParser>(
                        jobConfig,
                        (service) => service.Parse());
                    break;
                default:
                    break;
            }
        }

        private void AddOrUpdateRecurringJob<T>(
            HangfireJobConfigurationModel jobConfig,
            Expression<Func<T, Task>> methodCall) =>
                RecurringJob.AddOrUpdate(
                    jobConfig.JopType.ToString(),
                    methodCall,
                    jobConfig.CronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
    }
}
