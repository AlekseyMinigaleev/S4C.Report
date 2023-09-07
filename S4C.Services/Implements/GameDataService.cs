using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using S4C.YandexGateway.DeveloperPage;
using S4C.YandexGateway.DeveloperPage.Models;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameDataService"/>
    public class GameDataService : IGameDataService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly IMapper _mapper;
        private BaseLogger _logger;

        public GameDataService(
            IDeveloperPageGetaway developerPageGetaway,
            IMapper mapper,
            ReportDbContext dbContext)
        {
            _developerPageGetaway = developerPageGetaway;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task UpdateGameAndCreateGameStatisticRecord(
            PerformContext hangfireContext,
            CancellationToken cancellationToken)
        {
            _logger = new HangfireLogger(hangfireContext);

            var finalLogMessage = "Процесс успешно завершен.";
            var logErrorMessage = "Процесс завершен с ошибкой: ";
            var logLevel = LogLevel.Success;
            try
            {
                _logger.LogInformation("Начат процесс синхронизации всех данных по играм:");
                await RunAsync(cancellationToken);
            }
            catch (Exception e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = LogLevel.Error;
            }
            finally
            {
                _logger.Log(finalLogMessage, logLevel);
            }
        }

        private async Task RunAsync(
            CancellationToken cancellationToken)
        {
            var games = await _dbContext.Games
                .ToArrayAsync(cancellationToken);

            var gameIds = games
                .Select(x => x.Id)
                .ToArray();

            _logger.LogInformation($"Начало получения данных, количество игр: {gameIds.Length}.");
            var incomingGameData = await _developerPageGetaway
                .GetGameInfoAsync(gameIds, _logger, cancellationToken);
            _logger.LogSuccess($"Количество игр, по которым успешно получены данные: {gameIds.Length}.");

            _logger.LogInformation($"Начало обработки полученных данных:");
            await ProcessingIncomingDataAsync(incomingGameData, games, cancellationToken);
            _logger.LogSuccess($"Все данные успешно обработаны.");

            _logger.LogInformation($"Начало обновления базы данных.");
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"База данных успешно обновлена.");
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

                var (incomingGameModel,
                incomingGameStatisticModel) = Projection(incomingGameInfo);

                UpdateGameModel(
                    sourceGameModel,
                    incomingGameModel,
                    gameIdForLogs);

                _logger.LogInformation($"[{gameIdForLogs}] создана запись игровой статистики.");
                await _dbContext.GamesStatistics
                    .AddAsync(incomingGameStatisticModel, cancellationToken);
            }
        }

        private static (GameModel, string) GetDataForProcess(
            GameModel[] sourceGameModels,
            GameInfoModel incomingGameInfo)
        {
            var sourceGameModel = sourceGameModels
                .Single(x => x.Id == incomingGameInfo.AppId);

            var gameIdForLogs =
                sourceGameModel.Name is null
                    ? sourceGameModel.Id.ToString()
                    : sourceGameModel.Name;

            return (sourceGameModel, gameIdForLogs);
        }

        private (GameModel, GameStatisticModel) Projection(GameInfoModel incomingGameInfo)
        {
            var incomingGameModel = _mapper.Map<GameInfoModel, GameModel>(incomingGameInfo);
            var incomingGameStatisticModel = _mapper.Map<GameInfoModel, GameStatisticModel>(incomingGameInfo);

            SetLinksForStatuses(incomingGameInfo, incomingGameStatisticModel);

            return (incomingGameModel, incomingGameStatisticModel);
        }

        /*TODO: Сделать поддержку статуса promoted псоле реализации сервиса парсинга с РСЯ*/
        private void SetLinksForStatuses(GameInfoModel incomingGameInfo, GameStatisticModel incomingGameStatisticModel)
        {
            var existingGameStatusQuery = _dbContext.GameStatuses;
            var incomingGameStatusNames = incomingGameInfo.CategoriesNames;

            var gameStatusQuery = existingGameStatusQuery
                .Where(x => incomingGameStatusNames
                    .Contains(x.Name))
                .ToHashSet();

            incomingGameStatisticModel.AddStatuses(gameStatusQuery);
        }

        private void UpdateGameModel(GameModel sourceGame, GameModel incomingGame, string gameIdForLogs)
        {
            var hasChanges = sourceGame.HasChanges(incomingGame);

            if (hasChanges)
            {
                _logger.LogInformation($"[{gameIdForLogs}] данные актуальны.");
            }
            else
            {
                _logger.LogInformation($"[{gameIdForLogs}] есть изменения, установлена пометка на обновление.");
                sourceGame.Update(
                    incomingGame.Name!,
                    incomingGame.PublicationDate!.Value);
            }
        }
    }
}