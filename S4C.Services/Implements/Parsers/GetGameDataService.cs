using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Implements.Parsers.Exceptions;
using C4S.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace C4S.Services.Implements.Parsers
{
    public class GetGameDataService : IGetGameDataService
    {
        private readonly ReportDbContext _dbContext;
        private readonly ILogger<IGetGameDataService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /*TODO: вынести в бд, новая таблица с user settings*/
        private readonly string apiUrl = "https://yandex.ru/games/_crpd/2fxg324u1/0463a9o-i/WmeCEDy-VUd2S-vlZ9bXdhXct8xMhg5RScdAENGPZD0MTYp-CnAWBj63mxRiHCIVXFZeDaAw-_pk77fa1o_b6g4Xg239frXHxqdDTPsCJvJt3LvIFdsyWTgHsRSlg2gFVAn_fn8Fai4yy38QRgReUCh-OlGBcNfvWLe74sbqz5Zec-MdKb4RC2o7w6B3xtYebdzT9AWKalVo_1w4uaM0ZTRFyxZfGXY2fqdDaGoYZhAYXmJlpUjnxzjnm7qCgo-iAmfXZTHqURtCO-uoV9q6KgHwv7hZsgJROONgJLXfBAl0VbtaeyV2LmKrXxBiECIYEHJSPa0Ip_N4z8-CmsLHkg5vx31Bsh0LWj-buBuKzm45jOv8PZoyHTjHDAytsxw9IC2zIgc1ek4ugztIQgBeUEACDlHlYIuLZN_7_sb6s_5uG-M1WcYtV2Ybv7B31qZSAZiv_BH6MglQu3AklesQUVA1-3IDMQJM";

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
            var results = await GetAllGameDataAsync(gameIds);
            _logger.LogInformation($"получены {results.Count} из {gameIds.Length} игр");

            _logger.LogInformation($"начало обработки полученных игр");
            foreach (var result in results)
            {
                var game = games.Single(x => x.Id == result.Game.AppId);
                var gameIdForLogs =
                    game.Name is null
                        ? game.Id.ToString()
                        : game.Name;

                _logger.LogInformation($"[{gameIdForLogs}] обновления данных");
                UpdateGameModel(game, result.Game, gameIdForLogs);

                _logger.LogInformation($"[{gameIdForLogs}] создана запись игровой статистики");
                AddGameStatisticModel(game, result.GameStatistic);
            }

            _logger.LogInformation($"начало обновления базы данных");
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"база данных успешно обновлена");
        }

        private async Task<List<GameDataViewModel>> GetAllGameDataAsync(int[] gameIds)
        {
            var payload = GetPayload(gameIds);

            _logger.LogInformation($"посылается запрос на Yandex");
            var httpResponseMessage = await SendRequestAsync(payload);

            _logger.LogInformation($"ответ получен и начата обработка ответа от Yandex");
            var gameViewModels = await DeserializeObjectsAsync(httpResponseMessage, gameIds.Length);
            _logger.LogInformation($"ответ от Yandex успешно обработан");

            return gameViewModels;
        }

        private static StringContent GetPayload(int[] gameIds)
        {
            var requestData = new
            {
                appIDs = gameIds,
                format = "long"
            };

            string jsonPayload = JsonConvert.SerializeObject(requestData);

            var payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            return payload;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpContent httpContent)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(apiUrl, httpContent);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<List<GameDataViewModel>> DeserializeObjectsAsync(HttpResponseMessage httpResponseMessage, int gameCount)
        {
            var (gamesJToken, jsonString) = await GetGamesJTokenAndJsonStringAsync(httpResponseMessage);

            var results = new List<GameDataViewModel>();
            for (int i = 0; i < gameCount; i++)
            {
                var gameJToken = gamesJToken[i]
                ?? throw new InvalidContractException(jsonString, "Array is empty");

                var gameViewModel = GetGameViewModel(gameJToken, jsonString);

                var gameStatisticViewModel = GetGameStatisticViewModel(gameJToken, jsonString);

                var gameDataViewModel = new GameDataViewModel
                {
                    Game = gameViewModel,
                    GameStatistic = gameStatisticViewModel
                };

                results.Add(gameDataViewModel);
            }

            return results;
        }

        private async Task<(JToken, string)> GetGamesJTokenAndJsonStringAsync(HttpResponseMessage httpResponseMessage)
        {
            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();

            var jObject = JsonConvert
                .DeserializeObject<JObject>(jsonString)
                ?? throw new InvalidContractException(jsonString);

            var gamesJToken = jObject["games"]
                ?? throw new InvalidContractException(jsonString, "games");
            return (gamesJToken, jsonString);
        }

        private GameViewModel GetGameViewModel(JToken gameJToken, string jsonString)
        {
            var appId = (int?)gameJToken["appID"]
                ?? throw new InvalidContractException(jsonString, "appID");

            var name = (string?)gameJToken["title"]
                  ?? throw new InvalidContractException(jsonString, "title");

            var firstPublished = (int?)gameJToken["firstPublished"]
                ?? throw new InvalidContractException(jsonString, "firstPublished");

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
            var rating = (double?)gameJToken["rating"]
                  ?? throw new InvalidContractException(jsonString, "rating");

            var playersCount = (int?)gameJToken["playersCount"]
                ?? throw new InvalidContractException(jsonString, "playersCount");

            var categoriesNames = gameJToken["categoriesNames"]!.ToObject<string[]>()
                ?? throw new InvalidContractException(jsonString, "categoriesNames");

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
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name")]
        public int AppId { get; set; }

        [JsonProperty("firstPublished")]
        public int FirstPublished { get; set; }
    }

    public class GameStatisticViewModel
    {
        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("playersCount")]
        public int PlayersCount { get; set; }

        [JsonProperty("categoriesNames")]
        public string[] CategoriesNames { get; set; }
    }
}