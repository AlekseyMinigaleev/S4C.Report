using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Principal;
using С4S.API.Extensions;
using С4S.API.Models;

namespace С4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<ViewModel>
        {
            public Paginate Paginate { get; set; }
        }

        public class ViewModel
        {
            public GameViewModel[] Games { get; set; }

            public int TotalGamesCount { get; set; }
        }

        public class GameViewModel
        {
            public string Name { get; set; }

            public DateTime PublicationDate { get; set; }

            public double Evaluation { get; set; }

            public ValueWithProgress<int> PlayersCountWithProgress { get; set; }

            public ValueWithProgress<double?>? CashIncomeWithProgress { get; set; }
        }

        public class Handler : IRequestHandler<Query, ViewModel>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IPrincipal _principal;

            public Handler(
                ReportDbContext dbContext,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _principal = principal;
            }

            public async Task<ViewModel> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var gamesQuery = _dbContext.Games
                    .Where(x => x.UserId == userId)
                    /*TODO: дать возможность пользователю сортировать значения*/
                    .OrderByDescending(x => x.GameStatistics.Sum(game => game.PlayersCount))
                        .ThenByDescending(x => x.GameStatistics.Sum(game => game.CashIncome));

                var games = await gamesQuery
                    .Select(game => new GameViewModel
                    {
                        Name = game.Name!,

                        PublicationDate = game.PublicationDate!.Value,

                        PlayersCountWithProgress = new ValueWithProgress<int>(
                            game.GetPlayersCountActualValue(),
                            game.GetPlayersCountLastProgressValue()),

                        CashIncomeWithProgress = game.GameStatistics
                            .All(x => x.CashIncome == null)
                                ? null
                                : new ValueWithProgress<double?>(
                                    game.GetCashIncomeActualValue(),
                                    game.GetCashIncomeLastProgressValue()),

                        Evaluation = game.GameStatistics
                            .GetLastSynchronizationStatistic().Evaluation,
                    })

                    .Paginate(request.Paginate)
                    .ToArrayAsync(cancellationToken);

                var totalGamesCount = await gamesQuery
                  .CountAsync(cancellationToken);

                var response = new ViewModel
                {
                    Games = games,
                    TotalGamesCount = totalGamesCount,
                };

                return response;
            }
        }
    }



    /// <summary>
    /// Содержит методы-расширения для работы с моделью игры <see cref="GameModel"/>.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Получает актуальное значение количества игроков.
        /// </summary>
        /// <param name="source">Исходная модель игры.</param>
        /// <returns>Актуальное значение количества игроков.</returns>
        public static int GetPlayersCountActualValue(this GameModel source) =>
              source.GameStatistics
                .GetLastSynchronizationStatistic().PlayersCount;

        /// <summary>
        /// Получает последнее добавленное значение к актуальному количеству игроков.
        /// </summary>
        /// <param name="source">Исходная модель игры.</param>
        /// <returns>Значение, представляющее изменение количества игроков с предпоследней синхронизации.</returns>
        public static int GetPlayersCountLastProgressValue(this GameModel source) =>
            source.GetPlayersCountActualValue() - source.GameStatistics.GetBeforeLastSynchronizationStatistic().PlayersCount;

        /// <summary>
        /// Получает актуальное значение дохода.
        /// </summary>
        /// <param name="source">Исходная модель игры.</param>
        /// <returns>Актуальное значение дохода.</returns>
        public static double? GetCashIncomeActualValue(this GameModel source) =>
            source.GameStatistics
                .Select(x => x.CashIncome)
                .Sum();

        /// <summary>
        /// Получает последнее добавленное значение к актуальному доходу.
        /// </summary>
        /// <param name="source">Исходная модель игры.</param>
        /// <returns>Значение, представляющее изменение дохода с предпоследней синхронизации.</returns>
        public static double? GetCashIncomeLastProgressValue(this GameModel source) =>
            source.GameStatistics
                .GetLastSynchronizationStatistic().CashIncome;

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