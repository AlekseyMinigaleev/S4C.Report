using C4S.Services.Interfaces;
using MediatR;

namespace С4S.API.Features.Parsers
{
    public class GameStatisticParser
    {
        public class Query : IRequest<int>
        { }

        public class Handler : IRequestHandler<Query, int>
        {

            private readonly IParser _parser;
            public Handler(IParser parser )
            {
                _parser = parser;
            }

            public async Task<int> Handle(Query request, CancellationToken cancellationToken)
            {
                await _parser.ParseAsync();

                return 2;
            }
        }
    }
}
