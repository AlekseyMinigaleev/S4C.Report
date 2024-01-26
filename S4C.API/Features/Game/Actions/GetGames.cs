using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.Common.Models;
using C4S.DB;
using C4S.DB.Expressions;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Security.Principal;
using С4S.API.Extensions;
using С4S.API.Models;

namespace С4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<Response>
        {
            public Paginate Paginate { get; set; }

            public Sort Sort { get; set; }
        }

        public class Response
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
                    .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(GameExpressions.LastSynchronizedEvaluationExpression))
                    .ForMember(dest => dest.PlayersCountWithProgress, opt => opt.MapFrom(GameExpressions.PlayersCountWithProgressExpression))
                    .ForMember(dest => dest.CashIncomeWithProgress, opt => opt.MapFrom(GameExpressions.CashIncomeWithProgressExpression));
            }
        }

        public class Handler : IRequestHandler<Query, Response>
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

            public async Task<Response> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                /*
                 * TODO: Оптимизация запроса
                 * Не получается сортировать после проеции, если данные не материализованны.
                 * Сложно реализовать сортировку до проекции, возможно нужно полностью переделать Extension методы, которые исопльзуются в Expression
                 * Пока вижу только 1 вариант решения, в таблице Game хранить актуальные данные оценки, прибыл.
                 * Из за сроков не сиправил сразу.
                 */

                var games = await _dbContext.Games
                    .Where(x => x.UserId == userId)
                    .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken); 

                var sortedGames = games
                    .AsQueryable()
                    .OrderBy(request.Sort.GetSortExpression())
                    .Paginate(request.Paginate)
                    .ToArray();

                var response = new Response
                {
                    Games = sortedGames,
                    TotalGamesCount = games.Length
                };

                return response;
            }
        }
    }
}