using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using S4C.YandexGateway.DeveloperPageGateway;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameIdSyncService"/>
    public class GameIdSyncService : IGameIdSyncService
    {
        private readonly ILogger<GameIdSyncService> _logger;
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly ReportDbContext _dbcontext;

        public GameIdSyncService(
            ReportDbContext dbContext,
            ILogger<GameIdSyncService> logger,
            IDeveloperPageGetaway developerPageGetaway)
        {
            _dbcontext = dbContext;
            _logger = logger;
            _developerPageGetaway = developerPageGetaway;
        }

        /// <inheritdoc/>
        public async Task SyncAllGameIdAsync(
            CancellationToken cancellationToken = default)
        {
            var finalLogMessage = "процесс успешно завершен";
            var errorLogMessage = "процесс завершен c ошибкой: ";
            var warningLogMessage = "процесс завершен: ";
            var logLevel = LogLevel.Information;
            try
            {
                _logger.LogInformation($"Запущен процесс парсинга id игр");
                await RunAsync(cancellationToken);
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

        private async Task RunAsync(
            CancellationToken cancellationToken)
        {
            /*TODO: поправить логи*/
            _logger.LogInformation($"получение id игр");
            var gameIds = await _developerPageGetaway
                .GetGameIdsAsync(cancellationToken);
            _logger.LogInformation($"успешно получено id: {gameIds.Length}");

            /*TODO: поправить логи*/
            _logger.LogInformation($"начало обработки всех id");
            await ProcessingIncomingGameIdsAsync(gameIds, cancellationToken);
            _logger.LogInformation($"все id обработаны");
        }

        private async Task ProcessingIncomingGameIdsAsync(
            int[] gameIds,
            CancellationToken cancellationToken)
        {
            var countOfMissingGames = 0;
            foreach (var gameId in gameIds)
            {
                var increment = await ProcessingIncomingIdAsync(gameId, cancellationToken);
                countOfMissingGames += increment;
            }
            _logger.LogInformation($"начло добавления {countOfMissingGames}, записей в базу данных");
            await _dbcontext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"все записи успешно добавлены");
        }

        // этот метод возвращает инкремент для переменной countOfMissingGame
        private async Task<int> ProcessingIncomingIdAsync(
            int gameId,
            CancellationToken cancellationToken)
        {
            var existingGameIds = _dbcontext.GameModels
                .Select(x => x.Id);

            var isExistingGame = await existingGameIds
                .ContainsAsync(gameId, cancellationToken);

            if (!isExistingGame)
            {
                _logger.LogInformation($"[{gameId}] id игры не содержится в базе данных");
                var gameModel = new GameModel(gameId);
                await _dbcontext.GameModels
                    .AddAsync(gameModel, cancellationToken);
                _logger.LogInformation($"[{gameId}] id игры помечено на добавление в базу данных");

                // если игра не содержится в бд, увеличиваем countOfMissingGame
                return 1;
            }
            else
            {
                _logger.LogInformation($"[{gameId}] id игры уже содержится в базе данных");

                // если игра содержится в бд, не увеличиваем countOfMissingGame
                return 0;
            }
        }
    }
}