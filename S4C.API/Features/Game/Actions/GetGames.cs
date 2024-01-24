using C4S.DB;
using C4S.Helpers.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            public int PlayersCount { get; set; }

            /*TODO: Добавить Расчет этого поля. Текущее количество игроков - количество игроков на момент прошлой синхронизации*/
            public int PlayersGrowth { get; set; }

            public double Evaluation { get; set; }

            public double? CashIncome { get; set; }

            public double? CashIncomeGrowth { get; set; }
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
                    .Where(x => x.UserId == userId);

                var games = await gamesQuery
                    .Select(game => new GameViewModel
                    {
                        Name = game.Name!,

                        PublicationDate = game.PublicationDate!.Value,

                        PlayersCount = game.GameStatistics.First().PlayersCount,

                        Evaluation = game.GameStatistics.First().Evaluation,

                        CashIncome = game.GameStatistics.Sum(s => s.CashIncome),

                        CashIncomeGrowth = game.GameStatistics.First().CashIncome,
                    })
                    .OrderByDescending(x => x.PlayersCount)
                        .ThenByDescending(x => x.CashIncome)
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
}