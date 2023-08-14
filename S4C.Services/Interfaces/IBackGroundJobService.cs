using C4S.DB.Models.Hangfire;

namespace C4S.Services.Interfaces
{
    public interface IBackGroundJobService
    {
        public Task AddOrUpdateAllRecurringJobAsync(HangfireJobConfigurationModel? jobConfig = null);

        public Task AddOrUpdateRecurringJobAsync(HangfireJobConfigurationModel jobConfig);
    }
}
