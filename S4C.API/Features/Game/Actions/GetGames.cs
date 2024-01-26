using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Expressions;
using C4S.DB.Models;
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
            public string? Name { get; set; }

            public DateTime? PublicationDate { get; set; }

            public double Evaluation { get; set; }

            public ValueWithProgress<int> PlayersCountWithProgress { get; set; }

            public ValueWithProgress<double?>? CashIncomeWithProgress { get; set; }
        }

        public class GameViewModelProfiler : Profile
        {
            public GameViewModelProfiler()
            {
                CreateMap<GameModel, GameViewModel>()
                    .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(GameExpressions.GetLastSynchronizedEvaluation))
                    .ForMember(dest => dest.PlayersCountWithProgress, opt => opt.MapFrom(GameExpressions.GetPlayersCountWithProgress))
                    .ForMember(dest => dest.CashIncomeWithProgress, opt => opt.MapFrom(GameExpressions.GetCashIncomeWithProgress));
            }
        }

        public class Handler : IRequestHandler<Query, ViewModel>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IPrincipal _principal;
            private readonly IMapper _mapper;

            public Handler(
                ReportDbContext dbContext,
                 IMapper mapper,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _principal = principal;
                _mapper = mapper;
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
                        .ThenByDescending(x => x.GameStatistics.Sum(game => game.CashIncome))
                    .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider);

                var games = await gamesQuery
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