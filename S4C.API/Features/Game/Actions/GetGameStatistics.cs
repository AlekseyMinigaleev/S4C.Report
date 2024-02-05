using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using С4S.API.Extensions;
using С4S.API.Models;

namespace С4S.API.Features.Game.Actions
{
    public class GetGameStatistics
    {
        public class Query : IRequest<ResponseViewModel[]>
        {
            public Guid GameId { get; set; }

            public Paginate Paginate { get; set; }

            public Sort Sort { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Paginate)
                    .SetValidator(new PaginateValidator());
            }
        }

        public class ResponseViewModel
        {
            public double Evaluation { get; set; }

            public int PlayersCount { get; set; }

            public double? CashIncome { get; set; }

            public DateTime LastSynchroDate { get; set; }
        }

        public class ResponseViewModelProfiler : Profile
        {
            public ResponseViewModelProfiler()
            {
                CreateMap<GameStatisticModel, ResponseViewModel>();
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(
                ReportDbContext dbContext,
                IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<ResponseViewModel[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = await _dbContext.GamesStatistics
                    .Where(x => x.GameId == request.GameId)
                    .OrderBy(request.Sort.GetSortExpression())
                    .Paginate(request.Paginate)
                    .ProjectTo<ResponseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return response;
            }
        }
    }
}