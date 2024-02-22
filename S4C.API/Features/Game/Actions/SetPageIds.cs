using C4S.DB;
using C4S.Services.Services.GetGamesDataService;
using C4S.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;
using С4S.API.Extensions;

namespace С4S.API.Features.Game.Actions
{
    public class SetPageIds
    {
        public class Command : IRequest<ViewModel[]>
        {
            public Body[] Body { get; set; }
        }

        /*TODO: исправить после добавления фронта*/

        public class Body
        {
            /// <summary>
            /// Id игры
            /// </summary>
            public int GameId { get; set; }

            /// <summary>
            /// Навзание игры
            /// </summary>
            public string GameName { get; set; }

            /// <summary>
            /// Id страницы
            /// </summary>
            /// <remarks>
            /// Поле для взаимодействия с РСЯ
            /// </remarks>
            public int? PageId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(
                IPrincipal principal,
                ReportDbContext dbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(x => x)
                   .Must(x =>
                   {
                       var rsyaAuthorizationToken = principal.GetUserRsyaAuthorizationToken();
                       return rsyaAuthorizationToken is not null;
                   })
                   .WithErrorCode(HttpStatusCode.Unauthorized.ToString())
                   .WithMessage("Для использования возможности сбора статистики по прибыли игр, " +
                    "необходимо укзаать токен авторизации РСЯ");

                RuleForEach(x => x.Body)
                    .SetValidator(new BodyValidator(dbContext));
            }
        }

        public class BodyValidator : AbstractValidator<Body>
        {
            public BodyValidator(
                ReportDbContext dbContext)
            {
                RuleFor(x => x.GameId)
                    .MustAsync(async (gameId, cancellationToken) =>
                    {
                        var result = await dbContext.Games
                            .Select(x => x.AppId)
                            .ContainsAsync(gameId, cancellationToken);

                        return result;
                    })
                    .WithMessage(x => $"GameId: {x.GameId} не содержится в базе данных." +
                    $"Если вы уверены, что указанное id корректное" +
                    $"То проблема может быть решена принудительным запуском джобы 'Парсинг id игр со страницы разработчика'.");
            }
        }

        /*TODO: исправить после добавления фронта*/

        public class ViewModel
        {
            /// <summary>
            /// Id игры
            /// </summary>
            public int GameId { get; set; }

            /// <summary>
            /// Навзание игры
            /// </summary>
            public string GameName { get; set; }

            /// <summary>
            /// Id страницы
            /// </summary>
            /// <remarks>
            /// Поле для взаимодействия с РСЯ
            /// </remarks>
            public int? PageId { get; set; }

            /// <summary>
            /// Флаг показывающий было ли установлено значение
            /// </summary>
            public bool IsSuccessfullySet { get; set; }
        }

        public class Handler : IRequestHandler<Command, ViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IGetGameDataService _getGameDataService;
            private readonly IPrincipal _principal;

            public Handler(
                ReportDbContext dbContext,
                IGetGameDataService getGameDataService,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _getGameDataService = getGameDataService;
                _principal = principal;
            }

            public async Task<ViewModel[]> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var rsyaAuthorizationToken = _principal.GetUserRsyaAuthorizationToken();

                var period = new DateTimeRange(DateTime.Now, DateTime.Now);

                var response = new List<ViewModel>();
                foreach (var body in request.Body)
                {
                    bool isSuccessfullySet = false;
                    if (body.PageId.HasValue)
                    {
                        /*TODO: возможно нужно при наличии result, записывать его в бд*/
                        var result = await _getGameDataService
                            .GetPrivateGameDataAsync(
                                pageId: body.PageId.Value,
                                authorization: rsyaAuthorizationToken!,
                                period: period,
                                cancellationToken: cancellationToken);

                        isSuccessfullySet = result.CashIncome.HasValue;

                        if (isSuccessfullySet)
                            (await _dbContext.Games
                                .SingleAsync(x => x.AppId == body.GameId, cancellationToken))
                                .SetPageId(body.PageId.Value);
                    }

                    var viewModel = new ViewModel
                    {
                        GameId = body.GameId,
                        PageId = body.PageId,
                        GameName = body.GameName,
                        IsSuccessfullySet = isSuccessfullySet
                    };

                    response.Add(viewModel);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                return response.ToArray();
            }
        }
    }
}