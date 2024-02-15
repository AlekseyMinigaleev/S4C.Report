﻿using AngleSharp;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using С4S.API.Features.Authentication.ViewModels;
using С4S.API.Features.User.Requests;

namespace С4S.API.Features.Authentication.Actions
{
    public class CreateAccount
    {
        public class Query : IRequest
        {
            /// <inheritdoc cref="UserCredentionals"/>
            public UserCredentionals Credentionals { get; set; }

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
                get { return RsyaAuthorizationToken?.Token; }
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
                IBrowsingContext browsingContext,
                IHttpClientFactory httpClientFactory)
            {
                RuleLevelCascadeMode = CascadeMode.Stop;
                ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Credentionals)
                    .SetValidator(new UserCredentionalsValidator())
                    .Must(userCreditionals =>
                    {
                        var user = dbContext.Users
                            .SingleOrDefault(x => x.Login.Equals(userCreditionals.Login));

                        return user is null;
                    })
                    .WithMessage("Пользователь с указанным логином уже существует")
                    .WithErrorCode("login");

                RuleFor(x => x.DeveloperPageUrl)
                    .Must(developerPageUrl =>
                    {
                        var isValid = developerPageUrl
                            .StartsWith("https://yandex.ru/games/developer?name=");

                        /*TODO: хранить валидные ссылки в отдельной таблице, и сначала пытаться найти в этой таблице, только потом делать запрос, не парсить а запрос*/
                        if (!isValid)
                            return isValid;

                        var errorPage = browsingContext
                            .OpenAsync(developerPageUrl).Result
                            .QuerySelector(".error-page__title");

                        isValid = errorPage is null;

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
            private readonly ConsoleLogger _logger;

            public Handler(
                ReportDbContext dbContext,
                IHangfireBackgroundJobService hangfireBackgroundJobService,
                ILogger<CreateAccount> logger
                )
            {
                _dbContext = dbContext;
                _hangfireBackgroundJobService = hangfireBackgroundJobService;
                _logger = new ConsoleLogger(logger);
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
            }
        }
    }
}