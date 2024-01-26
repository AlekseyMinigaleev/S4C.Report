using C4S.DB.Extensions;
using C4S.DB.Models;
using System.Linq.Expressions;

namespace C4S.DB.Expressions
{
    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameModel"/>
    /// </summary>
    public static class GameExpressions
    {
        public static Expression<Func<GameModel, double>> GetLastSynchronizedEvaluation => (GameModel game) =>
            game.GameStatistics.GetLastSynchronizationStatistic().Evaluation;

        public static Expression<Func<GameModel, ValueWithProgress<int>>> GetPlayersCountWithProgress => (GameModel game) =>
            new ValueWithProgress<int>(
                game.GetPlayersCountActualValue(),
                game.GetPlayersCountLastProgressValue());

        public static Expression<Func<GameModel, ValueWithProgress<double?>?>> GetCashIncomeWithProgress => (GameModel game) =>
            game.GameStatistics.Any(gs => gs.CashIncome.HasValue)
                ? new ValueWithProgress<double?>(
                    game.GetCashIncomeActualValue(),
                    game.GetCashIncomeLastProgressValue())
                : null;
    }

    /// <summary>
    /// модель значения с прогрессом для сравнения.
    /// </summary>
    /// <typeparam name="T">Тип значения.</typeparam>
    public class ValueWithProgress<T> : IComparable<ValueWithProgress<T>>
    {
        /// <summary>
        /// Актуальное значение
        /// </summary>
        public T ActualValue { get; set; }

        /// <summary>
        /// Последнее добавленное значение к актуальному значению
        /// </summary>
        public T LastProgressValue { get; set; }

        /// <param name="actualValue">Актуальное значение.</param>
        /// <param name="lastProgressValue">Последнее добавленное значение к актуальному значению.</param>
        public ValueWithProgress(T actualValue, T lastProgressValue)
        {
            ActualValue = actualValue;
            LastProgressValue = lastProgressValue;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int CompareTo(ValueWithProgress<T>? obj)
        {
            if (obj is null)
                return 1;

            return Comparer<T>.Default.Compare(ActualValue, obj.ActualValue);
        }
    }
}