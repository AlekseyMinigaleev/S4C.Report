using C4S.Helpers.Logger;

namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис для синхронизации статусов игр
    /// </summary>
    public interface ICategoriesSyncService
    {
        /// <summary>
        /// Выполняет синхронизацию статусов игр с яндкс играми
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public Task SyncCategoriesAsync(BaseLogger logger, CancellationToken cancellationToken);
    }
}
