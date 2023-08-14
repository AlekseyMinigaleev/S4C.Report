using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using C4S.API.Features.Jobs.ViewModels;
using C4S.DB;

namespace C4S.API.Features.Jobs.Actions
{
    public class GetJobs
    {
        public class Query : IRequest<JobsViewModel[]>
        { }

        public class Handler : IRequestHandler<Query, JobsViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(ReportDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<JobsViewModel[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var jobs = await _dbContext.HangfireConfigurationModels
                    .ProjectTo<JobsViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();

                return jobs;
            }
        }
    }

}
