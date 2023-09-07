using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Helpers.Extensions;
using C4S.Helpers.Logger;
using C4S.Services.Exceptions;
using C4S.Services.Interfaces;
using Hangfire;
using Hangfire.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IHangfireBackgroundJobService"/>s
    public class BackgroundJobService : IHangfireBackgroundJobService
    {
        private readonly ReportDbContext _dbContext;

        public BackgroundJobService(ReportDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        /// <param name="updatedJobConfig" ><inheritdoc/></param>
        public async Task UpdateRecurringJobAsync(
            HangfireJobConfigurationModel updatedJobConfig,
            CancellationToken cancellationToken = default)
        {
            var existenceJobConfig = await _dbContext.HangfireConfigurations
                .SingleAsync(x => x.JobType == updatedJobConfig.JobType, cancellationToken);

            existenceJobConfig.Update(updatedJobConfig.CronExpression, updatedJobConfig.IsEnable);

            AddOrUpdateRecurringJob(existenceJobConfig);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddMissingHangfirejobsAsync(
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            var (existingJobConfigurations,
                 allJobTypes) = await GetSourcesAsync(cancellationToken);
            var localLogger = new AddMissingHangfireJobLogger(allJobTypes.Length, logger);

            localLogger.LogStart();

            var missingJobConfigList = GetAndProcessMissingJobConfigs(
                    allJobTypes,
                    existingJobConfigurations,
                    localLogger);

            await _dbContext.HangfireConfigurations.AddRangeAsync(missingJobConfigList, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            localLogger.LogFinish();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task OweriteJobsAsyncs(
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogInformation($"Запущен процесс перезаписи всех джоб в hangfire базе данных");
            var jobIds = Enum
                .GetValues(typeof(HangfireJobType))
                .Cast<HangfireJobType>()
                .Select(x => x.GetName())
                .ToArray();

            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection
                .GetRecurringJobs()
                .Where(x => !jobIds.Contains(x.Id))
                .ToList();

            recurringJobs.ForEach(x => RecurringJob.RemoveIfExists(x.Id));
            logger.LogInformation($"Все джобы удалены");

            var hangfireJobConfigurations = _dbContext.HangfireConfigurations;
            await hangfireJobConfigurations
                .ForEachAsync(AddOrUpdateRecurringJob, cancellationToken);
            logger.LogInformation($"Все джобы восстановлены");

            logger.LogSuccess($"Процесс успешно завершен, все джобы перезаписаны");
        }

        #region AddMissingHangfirejobsAsync Helpers

        private List<HangfireJobConfigurationModel> GetAndProcessMissingJobConfigs(
            HangfireJobType[] allJobTypes,
            HangfireJobConfigurationModel[] existingJobConfigurations,
            AddMissingHangfireJobLogger localLogger)
        {
            var missingJobConfigList = new List<HangfireJobConfigurationModel>();

            foreach (var processingJobType in allJobTypes)
            {
                var jobConfigOfProcessingType = existingJobConfigurations
                    .SingleOrDefault(x => x.JobType == processingJobType);

                var isMissingJobConfig = jobConfigOfProcessingType is null;
                if (isMissingJobConfig)
                {
                    var missingJobConfig = CreateMissingJobConfig(processingJobType);
                    missingJobConfigList.Add(missingJobConfig);
                    AddOrUpdateRecurringJob(missingJobConfig);
                }

                localLogger.Log(isMissingJobConfig, processingJobType.GetName());
            }

            return missingJobConfigList;
        }

        private async Task<(HangfireJobConfigurationModel[], HangfireJobType[])> GetSourcesAsync(
            CancellationToken cancellationToken)
        {
            // до конца не понятно что лучше, загружать все данные в память пользователя, или множественные запросы.
            // в связи с небольшого количества джоб,  было принято решение загружать данные в память пользователя.
            var existenceJobConfigurations = await _dbContext.HangfireConfigurations
                .ToArrayAsync(cancellationToken);

            var jobTypes = Enum
                .GetValues(typeof(HangfireJobType))
                .Cast<HangfireJobType>()
                .ToArray();

            return (existenceJobConfigurations, jobTypes);
        }

        private HangfireJobConfigurationModel CreateMissingJobConfig(HangfireJobType jobType)
        {
            var missingJobConfig = new HangfireJobConfigurationModel(
                       jobType: jobType,
                       cronExpression: HangfireJobConfigurationConstants.DefaultCronExpression,
                       isEnable: HangfireJobConfigurationConstants.DefaultIsEnable);

            return missingJobConfig;
        }

        private class AddMissingHangfireJobLogger
        {
            public int JobTypeCount { get; set; }
            public int ProcessedJobIndex { get; set; }
            public int CountJobWithDefaultConfig { get; set; }
            public string? LogPrefix { get; set; }

            public BaseLogger Logger { get; set; }

            public AddMissingHangfireJobLogger(int jobTypeCount, BaseLogger logger)
            {
                JobTypeCount = jobTypeCount;
                ProcessedJobIndex = 1;
                Logger = logger;
            }

            public void SetLogPrefix(string jobName)
            {
                LogPrefix = $"['{jobName}' ({ProcessedJobIndex}/{JobTypeCount})] ";
            }

            public void Log(bool isMissingJob, string jobName)
            {
                SetLogPrefix(jobName);

                if (isMissingJob)
                {
                    Logger.LogSuccess($"{LogPrefix}джоба пропущена, была выполнена регистрация со значениями по умолчанию");
                    CountJobWithDefaultConfig++;
                }
                else
                {
                    Logger.LogSuccess($"{LogPrefix}джоба уже зарегистрирована");
                }

                ProcessedJobIndex++;
            }

            public void LogStart()
            {
                Logger.LogInformation($"Запущен процесс регистрации недостающих джоб");
            }

            public void LogFinish()
            {
                Logger.LogSuccess($"Все джобы зарегистрированы. ({CountJobWithDefaultConfig}/{JobTypeCount}) со значением по умолчанию");
            }
        }

        #endregion AddMissingHangfirejobsAsync Helpers

        
        private void AddOrUpdateRecurringJob(HangfireJobConfigurationModel jobConfig)
        {
            switch (jobConfig.JobType)
            {
                case HangfireJobType.ParseGameIdsFromDeveloperPage:
                    AddOrUpdateRecurringJob<IGameIdSyncService>(
                        jobConfig,
                        (service) => service.SyncAllGameIdAsync(null, CancellationToken.None));
                    break;

                case HangfireJobType.SyncGameInfoAndGameCreateGameStatistic:
                    AddOrUpdateRecurringJob<IGameDataService>(
                        jobConfig,
                        (service) => service.UpdateGameAndCreateGameStatisticRecord(null, CancellationToken.None));
                    break;

                default:
                    break;
            }
        }

        /*TODO: если изменить имя в енамке, то изменения вступят в силу после пересоздания бд*/

        private void AddOrUpdateRecurringJob<T>(
            HangfireJobConfigurationModel jobConfig,
            Expression<Func<T, Task>> methodCall)
        {
            try
            {
                RecurringJob.AddOrUpdate(
                    jobConfig.JobType.GetName(),
                    methodCall,
                    jobConfig.CronExpression ?? HangfireJobConfigurationConstants.DefaultCronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
            }
            catch (InvalidOperationException)
            {
                var connectionString = _dbContext.Database.GetConnectionString();
                if (connectionString is null)
                    throw new HangfireDatabaseConnectException();
                else
                    throw new HangfireDatabaseConnectException(connectionString);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}