﻿using AutoMapper;
using C4S.Common.Models;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using S4C.YandexGateway.DeveloperPage;
using S4C.YandexGateway.DeveloperPage.Models;
using S4C.YandexGateway.RSYA;
using LogLevel = C4S.Helpers.Logger.LogLevel;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameDataService"/>
    public class GameDataService : IGameDataService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly IRsyaGateway _rsyaGateway;
        private readonly IMapper _mapper;
        private BaseLogger _logger;
        private UserModel _user;

        public GameDataService(
            IDeveloperPageGetaway developerPageGetaway,
            IRsyaGateway rsyaGateway,
            IMapper mapper,
            ReportDbContext dbContext)
        {
            _developerPageGetaway = developerPageGetaway;
            _rsyaGateway = rsyaGateway;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task SyncGameStatistics(
            Guid userId,
            PerformContext hangfireContext,
            CancellationToken cancellationToken)
        {
            _logger = new HangfireLogger(hangfireContext);
            await StartAsync(userId, cancellationToken);
        }

        /// <inheritdoc cref="IGameDataService.SyncGameStatistics(Guid, PerformContext, CancellationToken)"/>
        /// <remarks>
        /// Эта вариация выполняется не на стороне hangfire
        /// </remarks>
        public async Task SyncGameStatistics(
            Guid userId,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _logger = new ConsoleLogger(logger);
            await StartAsync(userId, cancellationToken);
        }

        private async Task StartAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            _user = await _dbContext.Users.SingleAsync(x => x.Id == userId, cancellationToken);

            var finalLogMessage = "Процесс успешно завершен.";
            var logErrorMessage = "Процесс завершен с ошибкой: ";
            var logLevel = LogLevel.Success;
            try
            {
                _logger.LogInformation("Начат процесс синхронизации всех данных по играм:");
                await Main(cancellationToken);
            }
            catch (Exception e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = LogLevel.Error;
                throw;
            }
            finally
            {
                _logger.Log(finalLogMessage, logLevel);
            }
        }

        private async Task Main(
            CancellationToken cancellationToken)
        {
            var games = await _dbContext.Games
                .Where(x => x.UserId == _user.Id)
                .ToArrayAsync(cancellationToken);

            var appIds = games
                .Select(x => x.AppId)
                .ToArray();

            var developerPagePrefix = "[Страница разработчика]";
            var rsyaPrefix = "[РСЯ]";
            _logger.LogInformation($"{developerPagePrefix} Начат процесс получения данных по всем играм.");
            var allIncomingGameData = await _developerPageGetaway
                .GetGamesInfoAsync(appIds, _logger, cancellationToken);
            _logger.LogSuccess($"{developerPagePrefix} Процесс успешно завершен");

            await StartEnrichGameInfoProcess(rsyaPrefix, allIncomingGameData, games);

            _logger.LogInformation($"Начало обработки полученных данных, со страницы разработчика:");
            await ProcessingIncomingDataAsync(allIncomingGameData, games, cancellationToken);
            _logger.LogSuccess($"Все данные успешно обработаны.");

            _logger.LogInformation($"Начало обновления базы данных.");
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"База данных успешно обновлена.");
        }

        private async Task StartEnrichGameInfoProcess(
            string rsyaPrefix,
            GameInfoModel[] allIncomingGameData,
            GameModel[] games)
        {
            _logger.LogInformation($"{rsyaPrefix} Начат процесс получения данных");
            var authorizationToken = _user.RsyaAuthorizationToken;
            if (authorizationToken is null)
            {
                _logger.LogWarning($"Отсутствует токен авторизации для РСЯ, процесс пропущен.");
            }
            else
            {
                await EnrichGameInfoProcess(allIncomingGameData, games, authorizationToken);
                _logger.LogSuccess($"{rsyaPrefix} Процесс завершен");
            }
        }

        private async Task EnrichGameInfoProcess(
            GameInfoModel[] gamesInfo,
            GameModel[] games,
            string authorizationToken)
        {
            var period = CreatePeriod(games.First());

            for (int i = 0; i < games.Length; i++)
            {
                var game = games[i];

                if (game.PageId.HasValue)
                {
                    var cashIncome = await _rsyaGateway.GetAppCashIncomeAsync(game.PageId.Value, authorizationToken, period);

                    if (!cashIncome.HasValue)
                        _logger.LogWarning($"Для игры '{game.Name}' неверно указан pageId. PageId: {game.PageId}");
                    else
                        _logger.LogSuccess($"Для игры '{game.Name}' доход успешно получен: {game.PageId}");
                    gamesInfo[i].CashIncome = cashIncome;
                }
                else
                {
                    _logger.LogWarning($"Для игры '{game.Name}' не указан pageId. Доход по этой игре не будет получен");
                }
            }
        }

        private DateTimeRange CreatePeriod(GameModel game)
        {
            var lastGameStatistic = _dbContext.GamesStatistics
                .Where(x => x.GameId == game.Id)
                .OrderBy(x => x.LastSynchroDate)
                .LastOrDefault();

            DateTime startDate = lastGameStatistic is null
                ? DateTime.Now
                : lastGameStatistic.LastSynchroDate.Date == DateTime.Now.Date
                    ? lastGameStatistic.LastSynchroDate.Date
                    : lastGameStatistic.LastSynchroDate.AddDays(1);

            var endDate = DateTime.Now;

            var period = new DateTimeRange(startDate, endDate);

            return period;
        }

        private async Task ProcessingIncomingDataAsync(
            GameInfoModel[] incomingGamesInfo,
            GameModel[] sourceGameModels,
            CancellationToken cancellationToken)
        {
            foreach (var incomingGameInfo in incomingGamesInfo)
            {
                var (sourceGameModel,
                    gameIdForLogs) = GetDataForProcess(sourceGameModels, incomingGameInfo);

                var (incomingGameModelFields,
                incomingGameStatisticModel) = Mapping(incomingGameInfo, sourceGameModel.Id);

                UpdateGameModel(
                    sourceGameModel,
                    incomingGameModelFields,
                    gameIdForLogs);

                _logger.LogInformation($"[{gameIdForLogs}] создана запись игровой статистики.");
                await _dbContext.GamesStatistics
                    .AddAsync(incomingGameStatisticModel, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private static (GameModel, string) GetDataForProcess(
            GameModel[] sourceGameModels,
            GameInfoModel incomingGameInfo)
        {
            var sourceGameModel = sourceGameModels
                .Single(x => x.AppId == incomingGameInfo.AppId);

            var gameIdForLogs =
                sourceGameModel.Name is null
                    ? sourceGameModel.AppId.ToString()
                    : sourceGameModel.Name;

            return (sourceGameModel, gameIdForLogs);
        }

        private (GameModel, GameStatisticModel) Mapping(GameInfoModel incomingGameInfo, Guid sourceGameId)
        {
            var incomingGameModifiableFields = _mapper.Map<GameInfoModel, GameModel>(incomingGameInfo);
            var incomingGameStatisticModel = _mapper.Map<GameInfoModel, GameStatisticModel>(incomingGameInfo);

            incomingGameStatisticModel.GameId = sourceGameId;

            SetLinksForCategories(incomingGameInfo, incomingGameModifiableFields);

            return (incomingGameModifiableFields, incomingGameStatisticModel);
        }

        private void SetLinksForCategories(
            GameInfoModel incomingGameInfo,
            GameModel gameModel)
        {
            var existCategories = _dbContext.Categories;

            var incomingCategories = incomingGameInfo.CategoriesNames;

            var categories = existCategories
                .Where(x => incomingCategories
                    .Contains(x.Name))
                .ToHashSet();

            gameModel.AddCategories(categories);
        }

        private void UpdateGameModel(
            GameModel sourceGame,
            GameModel incomingGameModifiableFields,
            string gameIdForLogs)
        {
            var hasChanges = sourceGame.HasChanges(incomingGameModifiableFields);

            if (hasChanges)
            {
                _logger.LogInformation($"[{gameIdForLogs}] есть изменения, установлена пометка на обновление.");

                /*TODO: убрать ! после фикса джобы*/
                sourceGame.Update(
                    name: incomingGameModifiableFields.Name!,
                    publicationDate: incomingGameModifiableFields.PublicationDate!.Value,
                    previewURL: incomingGameModifiableFields.PreviewURL!,
                    categories: incomingGameModifiableFields.Categories);
            }
            else
            {
                _logger.LogInformation($"[{gameIdForLogs}] данные актуальны.");
            }
        }
    }
}