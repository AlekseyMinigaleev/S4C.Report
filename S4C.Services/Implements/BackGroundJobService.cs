using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Services.Exceptions;
using C4S.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NCrontab;
using System.Linq.Expressions;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IBackGroundJobService"/>s
    public class BackGroundJobService : IBackGroundJobService
    {
        private readonly ReportDbContext _dbContext;

        public BackGroundJobService(ReportDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        /// <param name="updatedJobConfig" ><inheritdoc/></param>
        public async Task UpdateRecurringJobAsync(
            HangfireJobConfigurationModel updatedJobConfig,
            CancellationToken cancellationToken = default)
        {
            var existenceJobConfig = await _dbContext.HangfireConfigurationModels
                .SingleAsync(x => x.JobType == updatedJobConfig.JobType, cancellationToken);

            var (errorMessage, isValidCron) = IsValidCronExpression(updatedJobConfig.CronExpression);

            updatedJobConfig.NormalizeCronExpression();

            if (isValidCron)
                await UpdateRecurringJobAsync(updatedJobConfig, cancellationToken);
            else
                throw new InvalidCronExpressionException(updatedJobConfig.CronExpression);

            existenceJobConfig.Update(updatedJobConfig.CronExpression, updatedJobConfig.IsEnable);

            AddOrUpdateRecurringJob(existenceJobConfig);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddMissingHangfirejobsAsync(
            CancellationToken cancellationToken = default)
        {
            var existenceJobConfigurations = _dbContext.HangfireConfigurationModels;

            var jobTypes = Enum
                .GetValues(typeof(HangfireJobTypeEnum))
                .Cast<HangfireJobTypeEnum>();

            foreach (var jobType in jobTypes)
            {
                var existence = await existenceJobConfigurations
                    .SingleOrDefaultAsync(x => x.JobType == jobType, cancellationToken);

                if (existence is null)
                {
                    existence = new HangfireJobConfigurationModel(
                       jobType: jobType,
                       cronExpression: HangfireJobConfigurationConstants.DefaultCronExpression,
                       isEnable: HangfireJobConfigurationConstants.DefaultIsEnable);

                    _dbContext.HangfireConfigurationModels.Add(existence);
                    AddOrUpdateRecurringJob(existence);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        //TODO: уточнить, сейчас пользователь может оставить пустое значение для CronExpression и в таком случае IsEnable = false,
        //т.е.пользователь не должен иметь возможности седлать IsEnable = false и CronExpression = string.Empty?
        private static (string?, bool) IsValidCronExpression(string? cronExpression)
        {
            /*TODO: проверить случай с cronExpression = string.Empty*/
            var crontabSchedule = CrontabSchedule.TryParse(cronExpression);
            var result = crontabSchedule is null;

            return result
                  ? (null, result) // TODO: уточнить нужно ли сообщение, о том что c пустым CronExpression джоба всегда будет выключена
                  : ("Invalid cron expression", result);
        }

        private static void AddOrUpdateRecurringJob(HangfireJobConfigurationModel jobConfig)
        {
            switch (jobConfig.JobType)
            {
                case HangfireJobTypeEnum.ParseGameIdsFromDeveloperPage:
                    AddOrUpdateRecurringJob<IGameIdSyncService>(
                        jobConfig,
                        (service) => service.SyncAllGameIdAsync(null, CancellationToken.None));
                    break;

                case HangfireJobTypeEnum.SyncGameInfoAndGameCreateGameStatistic:
                    AddOrUpdateRecurringJob<IGameDataService>(
                        jobConfig,
                        (service) => service.UpdateGameAndCreateGameStatisticRecord(null, CancellationToken.None));
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
                    jobConfig.CronExpression ?? HangfireJobConfigurationConstants.DefaultCronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
    }
}