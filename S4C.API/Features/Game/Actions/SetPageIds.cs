using C4S.DB;
using C4S.DB.DTO;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using S4C.YandexGateway.RSYA;

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
            public int GameId { get; set; }

            public string GameName { get; set; }

            public int? PageId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Body>
        {
            private readonly ReportDbContext _dbContext;

            public CommandValidator(ReportDbContext dbContext)
            {
                _dbContext = dbContext;

                RuleFor(x => x.GameId)
                    .MustAsync(async (gameId, cancellationToken) =>
                    {
                        var result = await dbContext.Games
                            .Select(x => x.Id)
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
            public int GameId { get; set; }

            public int? PageId { get; set; }

            public string GameName { get; set; }

            public bool IsSuccessfullySet { get; set; }
        }

        public class Handler : IRequestHandler<Command, ViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IRsyaGateway _rsyaGateway;

            public Handler(
                ReportDbContext dbContext,
                IRsyaGateway rsyaGateway)
            {
                _dbContext = dbContext;
                _rsyaGateway = rsyaGateway;
            }

            public async Task<ViewModel[]> Handle(Command request, CancellationToken cancellationToken)
            {
                /*TODO: исправить после добавления авторизации*/
                var authorization = await _dbContext.Users
                    .Select(x => x.AuthorizationToken)
                    .SingleAsync(cancellationToken)
                    ?? throw new Exception("Отсутствует токен авторизации");

                var period = new DateTimeRange(DateTime.Now, DateTime.Now);

                var response = new List<ViewModel>();
                foreach (var body in request.Body)
                {
                    bool isSuccessfullySet = false;
                    if (body.PageId.HasValue)
                    {
                        var result = await _rsyaGateway
                            .GetAppCashIncomeAsync(
                                body.PageId.Value,
                                authorization,
                                period,
                                cancellationToken);

                        isSuccessfullySet = result.HasValue;

                        if (isSuccessfullySet)
                            (await _dbContext.Games
                                .SingleAsync(x => x.Id == body.GameId, cancellationToken))
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

                return response
                    .ToArray();
            }
        }
    }
}