using C4S.DB;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Features.User.Action
{
    public class SetRsyaAuthorizationToken
    {
        public class Command : IRequest<bool>
        {
            /// <summary>
            /// Токен авторизации для апи /partner2.yandex.ru/api
            /// </summary>
            public string AuthorizationToken { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IHttpClientFactory _httpClientFactory;
            private const string Url = "https://partner2.yandex.ru/api/statistics2/tree.json?lang=ru";

            public Handler(
                ReportDbContext dbContext,
                IHttpClientFactory httpClientFactory)
            {
                _dbContext = dbContext;
                _httpClientFactory = httpClientFactory;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                /*TODO: сделать отдельный метод для валиадции*/
                var httpRequestMethod = new HttpRequestMessage(HttpMethod.Get, Url);
                httpRequestMethod.Headers.Add("Authorization", request.AuthorizationToken);

                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.SendAsync(httpRequestMethod, cancellationToken);

                var result = response.IsSuccessStatusCode;

                if (result)
                {
                    /*TODO: Исправить после добавления авторизации*/
                    (await _dbContext.Users
                    .SingleAsync(cancellationToken))
                    .SetAuthorizationToken(request.AuthorizationToken);

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return result;
            }
        }
    }
}