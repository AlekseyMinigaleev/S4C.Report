using C4S.DB.Models.Hangfire;

namespace C4S.Services.Interfaces
{
    public interface IBackGroundJobService
    {
        public Task AddMissingHangfirejobsAsync(
            CancellationToken cancellationToken = default);

        public Task UpdateRecurringJobAsync(
            HangfireJobConfigurationModel jobConfig,
            CancellationToken cancellationToken = default);
    }
}