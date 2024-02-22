using C4S.DB.Models;

namespace C4S.DB.Extensions
{
    /// <summary>
    /// Справочник методов расширения для <see cref="GameModel"/>
    /// </summary>
    public static class GameExtensions
    {
        /// <summary>
        /// Получает актуальное значение дохода.
        /// </summary>
        /// <param name="source">Исходная модель игры.</param>
        /// <remarks>
        /// Возвращает <see langword="null"/> в случае если у <paramref name="source"/> не установлено поле <see cref="GameModel.PageId"/>
        /// </remarks>
        /// <returns>Актуальное значение дохода.</returns>
        public static double? GetCashIncomeActualValue(this GameModel source) =>
            source.GameStatistics
                .Select(x => x.CashIncome)
                .Sum();

        /// <summary>
        /// Получает последнее добавленное значение к актуальному доходу.
        /// </summary>
        /// <param name="source">Исходная модель игры.</param>
        /// <remarks>
        /// Возвращает <see langword="null"/> в случае если у <paramref name="source"/> не установлено поле <see cref="GameModel.PageId"/>
        /// </remarks>
        /// <returns>Значение, представляющее изменение дохода с предпоследней синхронизации.</returns>
        public static double? GetCashIncomeLastProgressValue(this GameModel source) =>
            source.GameStatistics
                .GetLastSynchronizationStatistic()?.CashIncome;

        /// <summary>
        /// Получает последнюю статистику синхронизации игры.
        /// </summary>
        /// <param name="source">Множество статистик игры.</param>
        /// <returns>Последняя статистика синхронизации.</returns>
        public static GameStatisticModel GetLastSynchronizationStatistic(this ISet<GameStatisticModel> source) => source
                .OrderByDescending(x => x.LastSynchroDate)
                .First();

        /// <summary>
        /// Получает статистику синхронизации перед последней.
        /// </summary>
        /// <param name="source">Множество статистик игры.</param>
        /// <returns>Статистика синхронизации перед последней.</returns>
        public static GameStatisticModel GetBeforeLastSynchronizationStatistic(this ISet<GameStatisticModel> source) => source
                .OrderByDescending(x => x.LastSynchroDate)
                .Take(2)
                .Last();
    }
}