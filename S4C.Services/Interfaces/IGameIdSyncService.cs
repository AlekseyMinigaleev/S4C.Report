using C4S.Helpers.Logger;
using Hangfire.Server;

namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Джоба выполняющая синхронизацию id игр между базой данных и Яндексом.,
    /// </summary>
    public interface IGameIdSyncService
    {
        /// <summary>
        ///  Выполняет процесс синхронизации id игр между базой данных и Яндексом.
        /// </summary>
        public Task SyncAllGameIdAsync(
            Guid userId,
            PerformContext hangfireContext = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///  Выполняет процесс синхронизации id игр между базой данных и Яндексом.
        /// </summary>
        /// <remarks>
        /// В этой вариации выполнения происходит не на стороне hangfire
        /// </remarks>
        public Task SyncAllGameIdAsync(
            Guid userId,
            BaseLogger logger,
            CancellationToken cancellationToken = default);
    }
}