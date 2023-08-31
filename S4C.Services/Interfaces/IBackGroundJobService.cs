using C4S.DB.Models.Hangfire;

namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис для управления Hangfires job
    /// </summary>
    public interface IBackGroundJobService
    {
        /// <summary>
        /// Выполняет создание недостающих джоб.
        /// </summary>
        /// <remarks>
        /// Работа метода включает в себя обновление таблицы <see cref="HangfireJobConfigurationModel"/>
        /// </remarks>
        public Task AddMissingHangfirejobsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполняет обновление указанной джобы.
        /// </summary>
        /// <remarks>
        /// Работа метода включает в себя обновление таблицы <see cref="HangfireJobConfigurationModel"/>
        /// </remarks>
        /// <param name="jobConfig">джоба которую нужно обновить</param>
        public Task UpdateRecurringJobAsync(
            HangfireJobConfigurationModel jobConfig,
            CancellationToken cancellationToken = default);
    }
}