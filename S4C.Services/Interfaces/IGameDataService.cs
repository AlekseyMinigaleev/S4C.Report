using C4S.DB.Models;
using Hangfire.Server;

namespace C4S.Services.Interfaces
{
    // по сути Game и GameStatistic это одна сущность представленная 2 таблицами и этот метод выполняет обновление данных этой сущности.
    /// <summary>
    /// Джоба выполняющая обновление данных в таблице <see cref="GameModel"/> и создание записей в таблице <see cref="GameStatisticModel"/>
    /// </summary>
    public interface IGameDataService
    {
        /// <summary>
        /// Выполняет обновление данных в таблице <see cref="GameModel"/> и создает новые записи для таблицы <see cref="GameStatisticModel"/>
        /// </summary>
        /// <remarks>
        /// Если у <see cref="GameModel"/> нет никаких изменений, то обновление данных пропускается.
        /// При успешном завершении процесса, создается по 1 записи в таблице <see cref="GameStatisticModel"/> для каждой <see cref="GameModel"/>
        /// </remarks>
        public Task UpdateGameAndCreateGameStatisticRecord(
            PerformContext hangfireContext,
            CancellationToken cancellationToken = default);
    }
}