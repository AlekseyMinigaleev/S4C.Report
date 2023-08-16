using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Helpers;
using C4S.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace C4S.Services.Implements.Parsers
{
    public class GameParser : IParser
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly ILogger<GameParser> _logger;
        private readonly ReportDbContext _dbcontext;
        private readonly IDetailedGameInfoParser _detailedGameInfoParser;

        private const string GameDeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";
        /*TODO: возможно это как то можно сделать через DI*/

        public GameParser(
            ReportDbContext dbContext,
            ILogger<GameParser> logger,
            IDetailedGameInfoParser detailedGameInfoParser)
        {
            _dbcontext = dbContext;
            _logger = logger;
            _detailedGameInfoParser = detailedGameInfoParser;

            var config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
        }

        public async Task ParseAsync()
        {
            // подразумевается 2 вида ошибок:
            // ArgumentNullException - все нормально это особенность конкретной страницы разработчика
            // Exception - баг, нужно вмешательство разработчика.
            var finalLogMessage = "Процесс успешно завершен";
            var logLevel = LogLevel.Information;
            try
            {
                _logger.LogInformation($"Запущен процесс парсинга игр");
                await Run();
            }
            catch (ArgumentNullException e)
            {
                finalLogMessage = $"процесс завершен:{e.Message}";
                logLevel = LogLevel.Warning;
            }
            catch (Exception e)
            {
                finalLogMessage = $"Процесс завершен с ошибкой: {e.Message}";
                logLevel = LogLevel.Error;
            }
            finally
            {
                _logger.Log(logLevel, finalLogMessage);
            }
        }

        private async Task Run()
        {
            _logger.LogInformation($"Начало получения игр как html элементов");
            var gameElements = await ParseGames(GameDeveloperPageUrl);
            _logger.LogInformation($"Получено элементов: {gameElements.Count()}");

            _logger.LogInformation($"Начало обработки всех элементов");
            await ElementsProcessing(gameElements);
            _logger.LogInformation($"все элементы обработаны");
        }

        private async Task ElementsProcessing(IHtmlCollection<IElement> gameElements)
        {
            var processedElementIndex = 1;
            foreach (var gameElement in gameElements)
            {
                _logger.LogInformation($"[{processedElementIndex}]: получение Id игры");
                var stringGameId = GetGameIdAsString(gameElement);
                _logger.LogInformation($"[{processedElementIndex}]: Id игры получено");

                _logger.LogInformation($"[{processedElementIndex}]: получение детальной информации об игре");
                _detailedGameInfoParser.SetUrl(stringGameId);
                var gameModel = _detailedGameInfoParser.GetDetailedGameInfo();
                _logger.LogInformation($"[{processedElementIndex}]: детальная информация об игре получена");

                await AddOrUpdateAsync(gameModel, processedElementIndex);

                _logger.LogInformation($"[{processedElementIndex}]: обработан");
            }

            /*TODO: проверить, обновится так или нет*/
            _logger.LogInformation($"[{processedElementIndex}]: начало обновления бд");
            await _dbcontext.SaveChangesAsync();
            _logger.LogInformation($"[{processedElementIndex}]: бд обновлена ");
        }

        private async Task<IHtmlCollection<IElement>> ParseGames(string gameDeveloperPageUrl)
        {
            var document = await _browsingContext.OpenAsync(gameDeveloperPageUrl);

            var gridList = document
                .QuerySelector(".grid-list")
                ?? throw new ArgumentNullException($"На странице {gameDeveloperPageUrl} нет игр");

            var children = gridList.Children;
            return children;
        }

        private string GetGameIdAsString(IElement element)
        {
            var gameUrlElement = element
                .QuerySelector(".game-url") as IHtmlAnchorElement;

            ParsersHelpers.ThrowIfNull(gameUrlElement, "У элемента отсутствует ссылка на игру");

            var path = gameUrlElement!.PathName;

            var gameId = GetId(path);

            return gameId;
        }

        private string GetId(string path)
        {
            var lastIndex = path.LastIndexOf("/");
            var gameId = path[(lastIndex + 1)..];
            return gameId;
        }

        private async Task AddOrUpdateAsync(GameModel game, int processingElementIndex)
        {
            var existingGame = await _dbcontext.GameModels.SingleOrDefaultAsync(x => x.Id == game.Id);

            if (existingGame is null)
            {
                _dbcontext.GameModels.Add(game);
                _logger.LogInformation($"[{processingElementIndex}] Игра помечена как добавляемая в бд");
            }
            else
            {
                existingGame.Update(game.Name, game.PublicationDate);
                _logger.LogInformation($"[{processingElementIndex}] Игра помечена как обновляемая в бд");
            }
        }
    }
}