using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using MediatR;

namespace С4S.API.Features.Jobs.Actions
{
    public class OweriteJobs
    {
        public class Command : IRequest
        { }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IHangfireBackgroundJobService _hangfireBackgroundJobService;
            private readonly ILogger<IHangfireBackgroundJobService> _iLogger;
            private readonly BaseLogger _logger;

            public Handler(
                IHangfireBackgroundJobService hangfireBackgroundJobService,
                ILogger<IHangfireBackgroundJobService> logger)
            {
                _hangfireBackgroundJobService = hangfireBackgroundJobService;
                _iLogger = logger;
                _logger = new ConsoleLogger<IHangfireBackgroundJobService>(_iLogger);
            }

            public async Task Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                await _hangfireBackgroundJobService.OweriteJobsAsyncs(_logger, cancellationToken);
            }
        }
    }
}