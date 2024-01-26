using C4S.Common.Models;
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
        /// <summary>
        /// Выражение для получения акутальной на данный момент оценки игры.
        /// </summary>
        public static Expression<Func<GameModel, double>> LastSynchronizedEvaluationExpression => (GameModel game) =>
            game.GameStatistics.GetLastSynchronizationStatistic().Evaluation;

        /// <summary>
        /// Выражение для получения количества игроков с прогрессом.
        /// </summary>
        public static Expression<Func<GameModel, ValueWithProgress<int>>> PlayersCountWithProgressExpression => (GameModel game) =>
            new ValueWithProgress<int>(
                game.GetPlayersCountActualValue(),
                game.GetPlayersCountLastProgressValue());

        /// <summary>
        /// Выражение для получения дохода с прогрессом.
        /// </summary>
        public static Expression<Func<GameModel, ValueWithProgress<double?>?>> CashIncomeWithProgressExpression => (GameModel game) =>
            game.GameStatistics.Any(gs => gs.CashIncome.HasValue)
                ? new ValueWithProgress<double?>(
                    game.GetCashIncomeActualValue(),
                    game.GetCashIncomeLastProgressValue())
                : null;
    }
}