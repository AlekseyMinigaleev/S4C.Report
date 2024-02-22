using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Services.BackgroundJobService;
using C4S.Services.Services.GameSyncService;
using C4S.Shared.Logger;
using C4S.Shared.Utils;
using FluentValidation;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;
using С4S.API.Features.Authentication.ViewModels;
using С4S.API.Features.User.Requests;

namespace С4S.API.Features.User.Actions
{
    public class CreateAccount
    {
        public class Query : IRequest
        {
            /// <inheritdoc cref="UserCredentials"/>
            public UserCredentials Credentionals { get; set; }

            /// <summary>
            /// ссылка на страницу разработчика
            /// </summary>
            public string DeveloperPageUrl { get; set; }

            /// <inheritdoc cref="RsyaAuthorizationToken"/>
            [JsonIgnore]
            public RsyaAuthorizationToken? RsyaAuthorizationToken { get; set; }

            [JsonPropertyName("rsyaAuthorizationToken")]
            public string? RsyaAuthorizationTokenString
            {
                get => RsyaAuthorizationToken?.Token;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        RsyaAuthorizationToken = null;
                    else
                        RsyaAuthorizationToken = new RsyaAuthorizationToken(value);
                }
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(
                ReportDbContext dbContext,
                IHttpClientFactory httpClientFactory)
            {
                RuleLevelCascadeMode = CascadeMode.Stop;
                ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Credentionals)
                    .SetValidator(new UserCredentionalsValidator())
                    .Must(userCredentials =>
                    {
                        var user = dbContext.Users
                            .SingleOrDefault(x => x.Login.Equals(userCredentials.Login));

                        return user is null;
                    })
                    .WithMessage("Пользователь с указанным логином уже существует")
                    .WithErrorCode("login");

                RuleFor(x => x.DeveloperPageUrl)
                    .Must(developerPageUrl =>
                    {
                        var keyword = "developer";
                        var index = developerPageUrl.IndexOf(keyword) + keyword.Length + 1; /*+1 учитывает слеш*/

                        var developerURL = developerPageUrl[..index];

                        var redirDataIndex = developerPageUrl.IndexOf("#redir-data");
                        string developerIdString;
                        if (redirDataIndex < 0)
                            developerIdString = developerPageUrl[index..];
                        else
                            developerIdString = developerPageUrl[index..redirDataIndex];

                        var parseResult = int.TryParse(developerIdString, out int developerId);

                        var isValid = developerURL
                            .StartsWith("https://yandex.ru/games/developer/") && parseResult;
                        if (!isValid)
                            return false;

                        var httpResponseMessage = HttpUtils
                            .SendRequestAsync(
                                 createRequest: () => new HttpRequestMessage(HttpMethod.Get, developerPageUrl),
                                httpClientFactory: httpClientFactory,
                                isEnsureSuccessStatusCode: false)
                        .Result;
                        isValid = httpResponseMessage.StatusCode != HttpStatusCode.NotFound;

                        return isValid;
                    })
                    .WithMessage("Указана не корректная ссылка на страницу разработчика")
                    .WithErrorCode("developerPageUrl");

                When(x => x.RsyaAuthorizationToken != null, () =>
                {
                    RuleFor(x => x.RsyaAuthorizationToken)
                        .SetValidator(new RsyaAuthorizationTokenValidator(httpClientFactory)!);
                });
            }
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IHangfireBackgroundJobService _hangfireBackgroundJobService;
            private readonly IGameSyncService _gameSyncService;
            private readonly ConsoleLogger _logger;

            public Handler(
                ReportDbContext dbContext,
                IHangfireBackgroundJobService hangfireBackgroundJobService,
                IGameSyncService gameSyncService,
                ILogger<CreateAccount> logger)
            {
                _dbContext = dbContext;
                _logger = new ConsoleLogger(logger);
                _hangfireBackgroundJobService = hangfireBackgroundJobService;
                _gameSyncService = gameSyncService;
            }

            public async Task Handle(Query request, CancellationToken cancellationToken)
            {
                var user = new UserModel(
                    login: request.Credentionals.Login,
                    password: request.Credentionals.Password,
                    developerPageUrl: request.DeveloperPageUrl,
                    rsyaAuthorizationToken: request.RsyaAuthorizationToken?.Token,
                    games: new HashSet<GameModel>());

                await _dbContext.Users.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await _hangfireBackgroundJobService
                    .AddMissingHangfirejobsAsync(user, _logger, cancellationToken);

                await _gameSyncService
                    .SyncGamesAsync(user.Id, _logger, cancellationToken);
            }
        }
    }
}