using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using S4C.YandexGateway.DeveloperPageGateway;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Models;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace C4S.Services.Implements
{
    public class GameDataService : IGameDataService
    {
        private readonly ReportDbContext _dbContext;
        private readonly ILogger<IGameDataService> _logger;
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly IMapper _mapper;

        public GameDataService(
            ILogger<IGameDataService> logger,
            IDeveloperPageGetaway developerPageGetaway,
            IMapper mapper,
            ReportDbContext dbContext)
        {
            _logger = logger;
            _developerPageGetaway = developerPageGetaway;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task GetAllGameDataAsync(
            CancellationToken cancellationToken)
        {
            var finalLogMessage = "процесс успешно завершен";
            var logErrorMessage = "процесс завершен с ошибкой: ";
            var logLevel = LogLevel.Information;
            try
            {
                _logger.LogInformation("начат процесс синхронизации всех данных по играм");
                await RunAsync(cancellationToken);
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

        private async Task RunAsync(
            CancellationToken cancellationToken)
        {
            var games = await _dbContext.GameModels
                .ToArrayAsync(cancellationToken);

            var gameIds = games
                .Select(x => x.Id)
                .ToArray();

            _logger.LogInformation($"получение данных, количество игр: {gameIds.Length}");
            var incomingGameData = await _developerPageGetaway
                .GetGameInfoAsync(gameIds, cancellationToken);
            _logger.LogInformation($"все данные успешно получены");

            _logger.LogInformation($"начало обработки полученных данных");
            await ProcessingIncomingDataAsync(incomingGameData, games, cancellationToken);

            _logger.LogInformation($"начало обновления базы данных");
            await _dbContext
                .SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"база данных успешно обновлена");
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
                    incomingGameInfoViewModel.GameI,
                    gameIdForLogs);

                _logger.LogInformation($"[{gameIdForLogs}] создана запись игровой статистики");
                await _dbContext.GamesStatisticModels
                    .AddAsync(incomingGameInfoViewModel.GameStatistic, cancellationToken);
            }
        }

        private IncomingGameInfoViewModel Projection(GameInfo incomingGameInfo)
        {
            var incomingGameModel = _mapper.Map<GameInfo, GameModel>(incomingGameInfo);
            var incomingGameStatisticModel = _mapper.Map<GameInfo, GameStatisticModel>(incomingGameInfo);

            var incomingGameInfoViewModel = new IncomingGameInfoViewModel(
                incomingGameModel,
                incomingGameStatisticModel);

            return incomingGameInfoViewModel;
        }

        private void UpdateGameModel(GameModel sourceGame, GameModel incomingGame, string gameIdForLogs)
        {
            /*TODO: вынести в спеку*/
            if (sourceGame.Name == incomingGame.Name
                && sourceGame.PublicationDate == incomingGame.PublicationDate)
            {
                _logger.LogInformation($"[{gameIdForLogs}] данные актуальны");
            }
            else
            {
                _logger.LogInformation($"[{gameIdForLogs}] есть изменения, установлена пометка на обновление");
                sourceGame.Update(
                    incomingGame.Name!,
                    incomingGame.PublicationDate!.Value);
            }
        }
    }

    public class IncomingGameInfoViewModel
    {
        public GameModel GameI { get; set; }
        public GameStatisticModel GameStatistic { get; set; }

        public IncomingGameInfoViewModel(
            GameModel gameInfo,
            GameStatisticModel gameStatisticInfo)
        {
            GameI = gameInfo;
            GameStatistic = gameStatisticInfo;
        }
    }
}