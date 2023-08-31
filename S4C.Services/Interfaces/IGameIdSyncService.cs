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
            CancellationToken cancellationToken = default);
    }
}