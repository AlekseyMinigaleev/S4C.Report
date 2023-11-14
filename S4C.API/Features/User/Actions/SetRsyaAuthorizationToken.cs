using C4S.DB;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using С4S.API.Features.User.Requests;

namespace С4S.API.Features.User.Action
{
    public class SetRsyaAuthorizationToken
    {
        public class Command : IRequest
        {
            /// <inheritdoc cref="SetRsyaAuthorizationToken"/>
            public RsyaAuthorizationToken RsyaAythorizationToken { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(IHttpClientFactory httpClientFactory)
            {
                RuleFor(x => x.RsyaAythorizationToken)
                    .SetValidator(new RsyaAuthorizationTokenValidator(httpClientFactory));
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ReportDbContext _dbContext;

            public Handler(ReportDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                /*TODO: Исправить после добавления авторизации*/
                (await _dbContext.Users
                .SingleAsync(cancellationToken))
                .SetAuthorizationToken(request.RsyaAythorizationToken.Token);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}