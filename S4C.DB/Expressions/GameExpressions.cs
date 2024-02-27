using C4S.DB.Extensions;
using C4S.DB.Models;
using C4S.Shared.Models;
using System.Linq.Expressions;

namespace C4S.DB.Expressions
{
    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameModel"/>
    /// </summary>
    public static class GameExpressions
    {
        /// <summary>
        /// Выражение для получения актуальной на данный момент оценки игры.
        /// </summary>
        public static Expression<Func<GameModel, double>> LastSynchronizedEvaluationExpression => (GameModel game) =>
            game.GameStatistics.GetLastSynchronizationStatistic()!.Evaluation;

        /*TODO: ValueWithProgress fix*/
        ///// <summary>
        ///// Выражение для получения дохода с прогрессом.
        ///// </summary>
        //public static Expression<Func<GameModel, ValueWithProgress<double?>?>> CashIncomeWithProgressExpression => (GameModel game) =>
        //   game.User.RsyaAuthorizationToken != null
        //    ? new ValueWithProgress<double?>(
        //        game.GetCashIncomeActualValue(),
        //        game.GetCashIncomeLastProgressValue())
        //    : null;
    }
}