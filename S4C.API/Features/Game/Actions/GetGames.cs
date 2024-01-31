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
        public class Query : IRequest<ResponseViewModel>
        {
        }

        public class ResponseViewModel
        {
            public GameViewModel[] Games { get; set; }

            public TotalViewModel Total => new()
            {
                PlayersCount = Games.Sum(x => x.PlayersCount?.ValueWithProgress?.ActualValue),
                CashIncome = Games.Sum(x => x.CashIncome?.ValueWithProgress?.ActualValue)
            };
        }

        public class TotalViewModel
        {
            public int? PlayersCount { get; set; }

            public double? CashIncome { get; set; }
        }

        public class GameViewModel
        {
            public string? Name { get; set; }

            public DateTime? PublicationDate { get; set; }

            public double Evaluation { get; set; }

            public PlayersCountViewModel PlayersCount { get; set; }

            public CashIncomeViewModel CashIncome { get; set; }
        }

        public class PlayersCountViewModel
        {
            public ValueWithProgress<int?>? ValueWithProgress { get; set; }

            public double? Percentage { get; set; }
        }

        public class CashIncomeViewModel
        {
            public ValueWithProgress<double?>? ValueWithProgress { get; set; }

            public double? Percentage { get; set; }
        }

        public class GameViewModelProfiler : Profile
        {
            public GameViewModelProfiler()
            {
                CreateMap<GameModel, GameViewModel>()
                    .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(GameExpressions.LastSynchronizedEvaluationExpression))
                    .ForMember(dest => dest.PlayersCount, opt => opt.MapFrom(src => src))
                    .ForMember(dest => dest.CashIncome, opt => opt.MapFrom(src => src))
                    ;

                CreateMap<GameModel, PlayersCountViewModel>()
                    .ForMember(dest => dest.ValueWithProgress, opt => opt.MapFrom(GameExpressions.PlayersCountWithProgressExpression))
                    .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => 0D));

                CreateMap<GameModel, CashIncomeViewModel>()
                    .ForMember(dest => dest.ValueWithProgress, opt => opt.MapFrom(GameExpressions.CashIncomeWithProgressExpression))
                    .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => 0D));
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel>
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

            public async Task<ResponseViewModel> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var games = await _dbContext.Games
                    .Where(x => x.UserId == userId)
                    .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                var response = new ResponseViewModel
                {
                    Games = games,
                };

                foreach (var game in response.Games)
                {
                    if (game.PlayersCount.ValueWithProgress is null)
                        game.PlayersCount.Percentage = null;
                    else
                    {
                        var acutal = (double)game.PlayersCount.ValueWithProgress.ActualValue!;
                        var a1 = (acutal / response.Total.PlayersCount);
                        var a2 = a1 * 100;

                        game.PlayersCount.Percentage = a2;
                    }

                    if (game.CashIncome.ValueWithProgress is null)
                        game.CashIncome.Percentage = null;
                    else
                    {
                        var a1 = (game.CashIncome.ValueWithProgress.ActualValue / response.Total.CashIncome);
                        var a2 = a1 * 100;

                        game.CashIncome.Percentage = a2;
                    }
                }

                return response;
            }
        }
    }
}