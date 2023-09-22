using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<ViewModel[]>
        { }

        public class ViewModel
        {
            public int GameId { get; set; }

            public string GameName { get; set; }

            public int? PageId { get; set; }
        }

        public class ViewModelProfiler : Profile
        {
            public ViewModelProfiler()
            {
                CreateMap<GameModel, ViewModel>()
                    .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.PageId))
                    .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Name));
            }
        }

        public class Handler : IRequestHandler<Query, ViewModel[]>
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

            public async Task<ViewModel[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await _dbContext.Games
                    .ProjectTo<ViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return result;
            }
        }
    }
}