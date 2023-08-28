using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Helpers;
using C4S.Services.Implements.Parsers.Exceptions;
using C4S.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace C4S.Services.Implements.Parsers
{
    public class GameIdParser : IParser
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly ILogger<GameIdParser> _logger;
        private readonly ReportDbContext _dbcontext;

        /*TODO: вынести в appsettings*/
        private const string GameDeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";

        public GameIdParser(
            ReportDbContext dbContext,
            ILogger<GameIdParser> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;

            /*TODO: как это сделать черзе DI?*/
            var config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
        }

        public async Task ParseAsync()
        {
            var finalLogMessage = "Процесс успешно завершен";
            var errorLogMessage = "процесс завершен c ошибкой: ";
            var warningLogMessage = "процесс завершен: ";
            var logLevel = LogLevel.Information;
            try
            {
                _logger.LogInformation($"Запущен процесс парсинга id игр");
                await Run();
            }
            catch (EmptyDeveloperPageException e)
            {
                finalLogMessage = $"{warningLogMessage}{e.Message}";
                logLevel = LogLevel.Warning;
            }
            catch (InvalidGameIdException e)
            {
                finalLogMessage = $"{errorLogMessage}{e.Message}";
                logLevel = LogLevel.Error;
            }
            catch (Exception e)
            {
                finalLogMessage = $"{errorLogMessage}{e.Message}";
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
            var gameElements = await ParseAllGames(GameDeveloperPageUrl);
            _logger.LogInformation($"Получено элементов: {gameElements.Count()}");

            _logger.LogInformation($"Начало обработки элементов");
            await ElementsProcessing(gameElements);
            _logger.LogInformation($"все элементы обработаны");
        }

        private async Task ElementsProcessing(IHtmlCollection<IElement> gameElements)
        {            
            var processedElementIndex = 1;
            var countOfMissingGames = 0;
            foreach (var gameElement in gameElements)
            {
                _logger.LogInformation($"[{processedElementIndex}] получение id");
                var stringGameId = GetGameIdAsString(gameElement);
                var isValidId = int.TryParse(stringGameId, out var gameId);

                if(isValidId)
                {
                    _logger.LogInformation($"[{processedElementIndex}] id получено");
                    var increment = await AddMissingGameIdAsync(gameId, processedElementIndex);
                    countOfMissingGames += increment;
                }
                else
                {
                    throw new InvalidGameIdException(stringGameId);
                }

                processedElementIndex++;
            }
            _logger.LogInformation($"начло добавления {countOfMissingGames}, записей в базу данных");
            await _dbcontext.SaveChangesAsync();
            _logger.LogInformation($"все записи успешно добавлены");
        }

        private async Task<IHtmlCollection<IElement>> ParseAllGames(string developerPageUrl)
        {
            var document = await _browsingContext
                .OpenAsync(developerPageUrl);

            var gridList = document
                .QuerySelector(".grid-list")
                ?? throw new EmptyDeveloperPageException(developerPageUrl);

            var children = gridList.Children;
            return children;
        }

        private static string GetGameIdAsString(IElement element)
        {
            var gameUrlElement = element
                .QuerySelector(".game-url") as IHtmlAnchorElement;

            ParsersHelpers.ThrowIfNull(gameUrlElement, "У элемента отсутствует ссылка на игру");

            var path = gameUrlElement!.PathName;

            var gameId = GetId(path);

            return gameId;
        }

        private static string GetId(string path)
        {
            var lastIndex = path.LastIndexOf("/");
            var gameId = path[(lastIndex + 1)..];
            return gameId;
        }

        // этот метод возвращает инкремент для переменной countOfMissingGame
        private async Task<int> AddMissingGameIdAsync(int gameId, int processedElementIndex)
        {
            var existingGameIds = _dbcontext.GameModels.Select(x => x.Id);
            var isExistingGame = await existingGameIds.ContainsAsync(gameId);

            if(!isExistingGame)
            {
                _logger.LogInformation($"[{processedElementIndex}] id игры не содержится в базе данных");
                var gameModel = new GameModel(gameId);
                await _dbcontext.GameModels.AddAsync(gameModel);
                _logger.LogInformation($"[{processedElementIndex}] id игры помечено на добавление в базу данных");

                // если игра не содержится в бд, увеличиваем countOfMissingGame
                return 1;
            }
            else
            {
                _logger.LogInformation($"[{processedElementIndex}] id игры уже содержится в базе данных");

                // если игра содержится в бд, не увеличиваем countOfMissingGame
                return 0;
            }
        }
    }
}