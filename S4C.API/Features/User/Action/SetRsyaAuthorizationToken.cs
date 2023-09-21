using C4S.DB;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Features.User.Action
{
    public class SetRsyaAuthorizationToken
    {
        public class Command : IRequest<bool>
        {
            public string AuthorizationToken { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IHttpClientFactory _httpClientFactory;
            private const string Url = "https://partner2.yandex.ru/api/statistics2/tree.json?lang=ru";

            public Handler(
                ReportDbContext dbContext, IHttpClientFactory httpClientFactory)
            {
                _dbContext = dbContext;
                _httpClientFactory = httpClientFactory;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var httpRequestMethod = new HttpRequestMessage(HttpMethod.Get, Url);
                httpRequestMethod.Headers.Add("Authorization", request.AuthorizationToken);

                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.SendAsync(httpRequestMethod);

                var result = response.IsSuccessStatusCode;

                if(result)
                {
                    (await _dbContext.Users
                    .SingleAsync(cancellationToken))
                    .SetAuthorizationToken(request.AuthorizationToken);

                    await _dbContext.SaveChangesAsync();
                }

                return result;
            }
        }
    }
}
