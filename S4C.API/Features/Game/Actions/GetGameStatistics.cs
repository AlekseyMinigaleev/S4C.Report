using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
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
            private readonly IPrincipal _principal;

            public Handler(
                ReportDbContext dbContext,
                IMapper mapper,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _principal = principal;
            }

            public async Task<ResponseViewModel[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();
                var response = await _dbContext.GamesStatistics
                    .Where(x => x.Game.UserId == userId)
                    .Paginate(request.Paginate)
                    .ProjectTo<ResponseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return response;
            }
        }
    }
}