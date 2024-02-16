using C4S.Helpers.Logger;
using C4S.Services.Implements;
using C4S.Services.Interfaces;
using MediatR;

namespace С4S.API.Features.Category.Actions
{
    public class SyncCategories
    {
        public class Command : IRequest
        { }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ICategoriesSyncService _categoriesSyncService;
            private readonly ConsoleLogger _logger;

            public Handler(
                ICategoriesSyncService categoriesSyncService,
                ILogger<SyncCategories> logger)
            {
                _categoriesSyncService = categoriesSyncService;
                _logger = new ConsoleLogger(logger);
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                await _categoriesSyncService.SyncCategoriesAsync(_logger, cancellationToken);
            }
        }
    }
}