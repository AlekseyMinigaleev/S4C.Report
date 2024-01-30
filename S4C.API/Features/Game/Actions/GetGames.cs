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

namespace С4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<IEnumerable<GameViewModel>>
        {
        }

        public class GameViewModel
        {
            public string? Name { get; set; }

            public DateTime? PublicationDate { get; set; }

            public double Evaluation { get; set; }

            public ValueWithProgress<int?>? PlayersCountWithProgress { get; set; }

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

        public class Handler : IRequestHandler<Query, IEnumerable<GameViewModel>>
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

            public async Task<IEnumerable<GameViewModel>> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var games = await _dbContext.Games
                    .Where(x => x.UserId == userId)
                    .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return games;
            }
        }
    }
}