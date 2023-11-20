using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace С4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<ViewModel[]>
        { }

        public class ViewModel
        {
            /// <summary>
            /// Id игры
            /// </summary>
            public int GameId { get; set; }

            /// <summary>
            /// Навзание игры
            /// </summary>
            public string GameName { get; set; }

            /// <summary>
            /// Id страницы
            /// </summary>
            /// <remarks>
            /// Поле для взаимодействия с РСЯ
            /// </remarks>
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
            private readonly IPrincipal _principal;

            public Handler(
                IPrincipal principal,
                ReportDbContext dbContext,
                IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _principal = principal;
            }

            public async Task<ViewModel[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var result = await _dbContext.Games
                    .Where(x => x.UserId == userId)
                    .ProjectTo<ViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return result;
            }
        }
    }
}