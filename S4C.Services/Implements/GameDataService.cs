using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.HangfireHelpers;
using C4S.Services.Interfaces;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using S4C.YandexGateway.DeveloperPageGateway;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Models;

/*
 * TODO: проверить как будут работать джобы если тыкать много раз на них. И если тыкать их в неправильном порядке.
 */
namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameDataService"/>
    public class GameDataService : IGameDataService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly IMapper _mapper;
        private IHangfireLogger _hangfireLogger;

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
            _hangfireLogger = new HangfireLogger(hangfireContext);

            var finalLogMessage = "Процесс успешно завершен.";
            var logErrorMessage = "Процесс завершен с ошибкой: ";
            var logLevel = HangfireLogLevel.Success;
            try
            {
                _hangfireLogger.LogInformation("Начат процесс синхронизации всех данных по играм:");
                await RunAsync(cancellationToken);
            }
            catch (HttpRequestException e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = HangfireLogLevel.Error;
            }
            catch (InvalidContractException e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = HangfireLogLevel.Error;
            }
            catch (Exception e)
            {
                finalLogMessage = $"{logErrorMessage}{e.Message}";
                logLevel = HangfireLogLevel.Error;
            }
            finally
            {
                _hangfireLogger.Log(finalLogMessage, logLevel);
            }
        }

        private async Task RunAsync(
            CancellationToken cancellationToken)
        {
            var games = await _dbContext.GameModels
                .ToArrayAsync(cancellationToken);

            var gameIds = games
                .Select(x => x.Id)
                .ToArray();

            _hangfireLogger.LogInformation($"Начало получения данных, количество игр: {gameIds.Length}.");
            var incomingGameData = await _developerPageGetaway
                .GetGameInfoAsync(gameIds, cancellationToken);
            _hangfireLogger.LogSuccess($"Количество игр, по которым успешно получены данные: {gameIds.Length}.");

            _hangfireLogger.LogInformation($"Начало обработки полученных данных:");
            await ProcessingIncomingDataAsync(incomingGameData, games, cancellationToken);
            _hangfireLogger.LogSuccess($"Все данные успешно обработаны.");

            _hangfireLogger.LogInformation($"Начало обновления базы данных.");
            await _dbContext
                .SaveChangesAsync(cancellationToken);
            _hangfireLogger.LogSuccess($"База данных успешно обновлена.");
        }

        private async Task ProcessingIncomingDataAsync(
            GameInfo[] incomingGamesInfo,
            GameModel[] sourceGameModels,
            CancellationToken cancellationToken)
        {
            foreach (var incomingGameInfo in incomingGamesInfo)
            {
                var sourceGameModel = sourceGameModels
                    .Single(x => x.Id == incomingGameInfo.AppId);

                var gameIdForLogs =
                    sourceGameModel.Name is null
                        ? sourceGameModel.Id.ToString()
                        : sourceGameModel.Name;

                var incomingGameInfoViewModel = Projection(incomingGameInfo);

                UpdateGameModel(
                    sourceGameModel,
                    incomingGameInfoViewModel.Game,
                    gameIdForLogs);

                _hangfireLogger.LogInformation($"[{gameIdForLogs}] создана запись игровой статистики.");
                await _dbContext.GamesStatisticModels
                    .AddAsync(incomingGameInfoViewModel.GameStatistic, cancellationToken);
            }
        }

        private ProjectedGameInfoViewModel Projection(GameInfo incomingGameInfo)
        {
            /*TODO: сделать мапинг срау в ProjectedGameInfoViewModel, вынести вм в отдельный файл*/
            var incomingGameModel = _mapper.Map<GameInfo, GameModel>(incomingGameInfo);
            var incomingGameStatisticModel = _mapper.Map<GameInfo, GameStatisticModel>(incomingGameInfo);

            var projectedGameInfoViewModel = new ProjectedGameInfoViewModel(
                incomingGameModel,
                incomingGameStatisticModel);

            return projectedGameInfoViewModel;
        }

        private void UpdateGameModel(GameModel sourceGame, GameModel incomingGame, string gameIdForLogs)
        {
            /*TODO: вынести в спеку*/
            if (sourceGame.Name == incomingGame.Name
                && sourceGame.PublicationDate == incomingGame.PublicationDate)
            {
                _hangfireLogger.LogInformation($"[{gameIdForLogs}] данные актуальны.");
            }
            else
            {
                _hangfireLogger.LogInformation($"[{gameIdForLogs}] есть изменения, установлена пометка на обновление.");
                sourceGame.Update(
                    incomingGame.Name!,
                    incomingGame.PublicationDate!.Value);
            }
        }
    }

    /// <summary>
    /// Модель представляющая данные <see cref="GameInfo"/>, подготовленные для обновления базы данных
    /// </summary>
    public class ProjectedGameInfoViewModel
    {
        /// <summary>
        /// Данные <see cref="GameInfo"/>, подготовленные для обновления таблицы <see cref="GameModel"/>
        /// </summary>
        public GameModel Game { get; set; }

        /// Данные <see cref="GameInfo"/>, подготовленные для обновления таблицы <see cref="GameStatistic"/>
        public GameStatisticModel GameStatistic { get; set; }

        /// <param name="gameInfo">
        /// Данные <see cref="GameInfo"/>, подготовленные для обновления таблицы <see cref="GameStatistic"/>
        /// </param>
        /// <param name="gameStatisticInfo">
        /// Данные <see cref="GameInfo"/>, подготовленные для обновления таблицы <see cref="GameModel"/>
        /// </param>
        public ProjectedGameInfoViewModel(
            GameModel gameInfo,
            GameStatisticModel gameStatisticInfo)
        {
            Game = gameInfo;
            GameStatistic = gameStatisticInfo;
        }
    }
}