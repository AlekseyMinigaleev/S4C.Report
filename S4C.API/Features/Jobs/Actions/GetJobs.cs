using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using Microsoft.OpenApi.Extensions;

namespace C4S.API.Features.Jobs.Actions
{
    public class GetJobs
    {
        public class Query : IRequest<ResponseViewModel[]>
        { }

        public class ResponseViewModel
        {
            public string Name { get; set; }

            public HangfireJobTypeEnum JobType { get; set; }

            public string CronExpression { get; set; }

            public string Frequency { get; set; }

            public bool IsEnable { get; set; }
        }

        public class ResponseViewModelProfiler : Profile
        {
            public ResponseViewModelProfiler()
            {
                CreateMap<HangfireJobConfigurationModel, ResponseViewModel>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.JobType.GetDisplayName()));
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(ReportDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<ResponseViewModel[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var jobs = await _dbContext.HangfireConfigurationModels
                    .ProjectTo<ResponseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();

                return jobs;
            }
        }
    }

}
