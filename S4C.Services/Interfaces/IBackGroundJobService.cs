using C4S.DB.Models.Hangfire;

namespace C4S.Services.Interfaces
{
    public interface IBackGroundJobService
    {
        public Task AddOrUpdateRecurringJobAsync(HangfireJobConfigurationModel? jobConfig);
    }
}
