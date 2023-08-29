using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace C4S.Services.Implements.Parsers
{
    public class GetGameDataService : IGetGameDataService
    {
        private readonly ReportDbContext _dbContext;
        private readonly ILogger<IGetGameDataService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetGameDataService(
            IHttpClientFactory httpClientFactory,
            ILogger<IGetGameDataService> logger,
            ReportDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task GetAllGameDataAsync()
        {
            var finalLogMessage = "процесс успешно завершен";
            var logErrorMessage = "процесс завершен с ошибкой: ";
            var logLevel = LogLevel.Information;
            try
            {
                _logger.LogInformation("начат процесс синхронизации всех данных по играм");
                await RunAsync();
            }
            catch (HttpRequestException e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = LogLevel.Error;
            }
            catch (InvalidContractException e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = LogLevel.Error;
            }
            catch (Exception e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = LogLevel.Error;
            }
            finally
            {
                _logger.Log(logLevel, finalLogMessage);
            }
        }

        private async Task RunAsync()
        {
            var games = await _dbContext.GameModels
                .ToArrayAsync();

            var gameIds = games
                .Select(x => x.Id)
                .ToArray();

            _logger.LogInformation($"начато получение данных по {games.Length} играм");
            var incomingGameData = await GetAllGameDataAsync(gameIds);
            _logger.LogInformation($"получены {incomingGameData.Count} из {gameIds.Length} игр");

            _logger.LogInformation($"начало обработки полученных игр");
            ProcessingIncomingData(incomingGameData, games);

            _logger.LogInformation($"начало обновления базы данных");
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"база данных успешно обновлена");
        }
        
        private void ProcessingIncomingData(List<GameDataViewModel> incomingGames, GameModel[] sourceGames)
        {
            foreach (var result in incomingGames)
            {
                var game = sourceGames.Single(x => x.Id == result.Game.AppId);
                var gameIdForLogs =
                    game.Name is null
                        ? game.Id.ToString()
                        : game.Name;

                _logger.LogInformation($"[{gameIdForLogs}] обновления данных");
                UpdateGameModel(game, result.Game, gameIdForLogs);

                _logger.LogInformation($"[{gameIdForLogs}] создана запись игровой статистики");
                AddGameStatisticModel(game, result.GameStatistic);
            }
        }



       

       

        private GameViewModel GetGameViewModel(JToken gameJToken, string jsonString)
        {
           
            var gameViewModel = new GameViewModel
            {
                AppId = appId,
                Name = name,
                FirstPublished = firstPublished,
            };
            return gameViewModel;
        }

        private GameStatisticViewModel GetGameStatisticViewModel(JToken gameJToken, string jsonString)
        {
           

            var gameStatisticViewModel = new GameStatisticViewModel
            {
                Rating = rating,
                PlayersCount = playersCount,
                CategoriesNames = categoriesNames
            };

            return gameStatisticViewModel;
        }

        private void UpdateGameModel(GameModel game, GameViewModel gameViewModel, string gameIdForLogs)
        {
            var publicationDate = DateTimeOffset
                .FromUnixTimeSeconds(gameViewModel.FirstPublished)
                .DateTime;

            if (game.Name == gameViewModel.Name && game.PublicationDate == publicationDate)
            {
                _logger.LogInformation($"[{gameIdForLogs}] данные актуальны");
            }
            else
            {
                _logger.LogInformation($"[{gameIdForLogs}] ecть изменения, установлена пометка на обновление");
                game.Update(gameViewModel.Name, publicationDate);
            }
        }

        private void AddGameStatisticModel(GameModel game, GameStatisticViewModel gameStatisticViewModel)
        {
            /*TODO:вынести категории в таблицу*/
            var isNew = gameStatisticViewModel.CategoriesNames
                .Contains("new");
            var isPromoted = gameStatisticViewModel.CategoriesNames
                .Contains("promoted ");

            var lasSynchroDate = DateTime.Now;

            var gameStatistic = new GamesStatisticModel(
                game,
                gameStatisticViewModel.PlayersCount,
                isNew,
                isPromoted,
                lasSynchroDate,
                gameStatisticViewModel.Rating);

            _dbContext.GamesStatisticModels.Add(gameStatistic);
        }
    }

    public class GameDataViewModel
    {
        public GameViewModel Game { get; set; }

        public GameStatisticViewModel GameStatistic { get; set; }
    }

    public class GameViewModel
    {

    }

    public class GameStatisticViewModel
    {

    }
}