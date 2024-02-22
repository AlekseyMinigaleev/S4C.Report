using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Expressions;
using C4S.DB.Models;
using C4S.Shared.Extensions;
using C4S.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Security.Principal;

namespace С4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<ResponseViewModel>
        { }

        public class ResponseViewModel
        {
            public GameViewModel[] Games { get; set; }

            public TotalViewModel Total => new()
            {
                CashIncome = Games
                    .Any(x => x.CashIncome.ValueWithProgress is not null)
                        ? Games.Sum(x => x.CashIncome.ValueWithProgress?.ActualValue)
                        : null
            };
        }

        public class TotalViewModel
        {
            public double? CashIncome { get; set; }
        }

        public class CashIncomeViewModel
        {
            public ValueWithProgress<double?>? ValueWithProgress { get; set; }
            public double? Percentage { get; set; }
        }

        public class GameViewModel
        {
            public Guid Id { get; set; }

            public string? Name { get; set; }

            public DateTime? PublicationDate { get; set; }

            public double Evaluation { get; set; }

            public string URL { get; set; }

            public string PreviewURL { get; set; }

            public string[] Categories { get; set; }

            public CashIncomeViewModel CashIncome { get; set; }
        }

        public class GameViewModelProfiler : Profile
        {
            public GameViewModelProfiler()
            {
                CreateMap<GameModel, GameViewModel>()
                    .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(GameExpressions.LastSynchronizedEvaluationExpression))
                    .ForMember(dest => dest.CashIncome, opt => opt.MapFrom(src => src))
                    .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryGameModels
                        .Select(x => x.Category.Title)));

                CreateMap<GameModel, CashIncomeViewModel>()
                  .ForMember(dest => dest.ValueWithProgress, opt => opt.MapFrom(GameExpressions.CashIncomeWithProgressExpression))
                  .ForMember(dest => dest.Percentage, opt => opt.Ignore());
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
                    .Include(x => x.User) /*Почему то не подргужает автоматически для получения URL*/
                    .Where(x => x.UserId == userId)
                    .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                var response = new ResponseViewModel
                {
                    Games = games,
                };

                EnrichResponseWithPercentage(response);

                return response;
            }

            private static void EnrichResponseWithPercentage(ResponseViewModel response)
            {
                foreach (var game in response.Games)
                {
                    /*TODO: делать percentage по рейтингу?*/
                    if (game.CashIncome.ValueWithProgress is not null
                        && game.CashIncome.ValueWithProgress.ActualValue is not null)
                    {
                        game.CashIncome.Percentage = CalculatePercentage(
                            game.CashIncome.ValueWithProgress.ActualValue.Value,
                            response.Total.CashIncome!.Value);
                    }
                }
            }

            private static double CalculatePercentage<T>(T value, T total)
                where T : IConvertible
            {
                var a = Convert.ToDouble(value);
                var b = Convert.ToDouble(total);

                double percentage = (a / b) * 100;

                var roundedPercentage = Math.Round(percentage, 3);

                return roundedPercentage;
            }
        }
    }
}