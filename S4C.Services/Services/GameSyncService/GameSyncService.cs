using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Services.GameSyncService.Helpers;
using C4S.Services.Services.GetGamesDataService;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Extensions;
using C4S.Shared.Logger;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Services.GameSyncService
{
    /// <inheritdoc cref="IGameSyncService"/>
    public class GameSyncService : IGameSyncService
    {
        private readonly ReportDbContext _dbContext;
        private readonly GameModelEnricher _gameModelEnricher;
        private readonly IGetGameDataService _getGameDataService;
        private readonly IMapper _mapper;

        private BaseLogger _logger;
        private UserModel _user;
        private IQueryable<GameModel> _existingGameModelsQuery;

        public GameSyncService(
            ReportDbContext dbContext,
            GameModelEnricher gameModelEnricher,
            IGetGameDataService getGameDataService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _getGameDataService = getGameDataService;
            _gameModelEnricher = gameModelEnricher;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task SyncGamesAsync(
            Guid userId,
            PerformContext hangfireContext,
            CancellationToken cancellationToken)
        {
            var hangfireLogger = new HangfireLogger(hangfireContext);

            await SyncGamesAsync(
                userId,
                hangfireLogger,
                cancellationToken);
        }

        /// <inheritdoc/>
        public async Task SyncGamesAsync(
            Guid userId,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            _logger = logger;
            _user = GetUser(userId);
            _existingGameModelsQuery = _dbContext.Games
                .Where(x => x.UserId == _user.Id);

            _logger.LogInformation("Начат процесс получения данных по всем играм");
            var publicGamesData = await _getGameDataService.GetPublicGameDataAsync(
                _user.DeveloperPageUrl,
                _logger,
                cancellationToken);
            _logger.LogInformation("Обработка полученных данных");

            var newGameModels = new List<GameModel>();
            foreach (var publicGameData in publicGamesData)
            {
                var newGameModel = _mapper.Map<PublicGameData, GameModel>(publicGameData);
                var newGameStatistic = _mapper.Map<PublicGameData, GameStatisticModel>(publicGameData);
                newGameModel.SetUser(_user);

                newGameModel.GameStatistics = new HashSet<GameStatisticModel>() { newGameStatistic };

                await _gameModelEnricher.EnrichGameModelsAsync(
                    gameModelToEnrich: newGameModel,
                    publicGameData: publicGameData,
                    cancellationToken: cancellationToken);

                newGameModels.Add(newGameModel);
            }
            _logger.LogSuccess("Данные успешно обработаны");

            _logger.LogInformation("Сохранение данных в бд");
            await UpdateDatabaseAsync(newGameModels, cancellationToken);
            _logger.LogSuccess("Данные успешно сохранены");

            _logger.LogInformation("Процесс получения данных по всем играм, успешно завершен");
        }

        private UserModel GetUser(Guid userId)
        {
            var user = _dbContext.Users
                .SingleOrDefault(x => x.Id == userId);

            if (user is null)
                throw new ArgumentNullException($"{nameof(user)} Пользователь с указанным Id не был найден.");

            return user;
        }

        private async Task UpdateDatabaseAsync(IEnumerable<GameModel> newGameModels, CancellationToken cancellationToken)
        {
            var existingGameModels = _existingGameModelsQuery
                .Include(x => x.CategoryGameModels)
                    .ThenInclude(x => x.Category)
                .ToList();

            Archive(existingGameModels, newGameModels);

            Add(existingGameModels, newGameModels);

            Update(existingGameModels, newGameModels);

            _logger.LogInformation($"Начало обновление бд");
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"Бд обновлена");

            void Archive(
                IEnumerable<GameModel> existingGameModels,
                IEnumerable<GameModel> newGameModels)
            {
                var gameModelsToArchive = existingGameModels
                    .GetItemsNotInSecondCollection(newGameModels)
                    .ToList();

                gameModelsToArchive
                    .ForEach(x => x.SetIsArchived(true));

                _logger.LogInformation($"Пометка 'архивные' установлена {gameModelsToArchive.Count} играм");
            }

            void Add(
                IEnumerable<GameModel> existingGameModels,
                IEnumerable<GameModel> newGameModels)
            {
                var gameModelsToAdd = newGameModels
                   .GetItemsNotInSecondCollection(existingGameModels);

                _dbContext.Games.AddRange(gameModelsToAdd);
                _logger.LogInformation($"На добавление помечено {gameModelsToAdd.Count()} игр");
            }

            void Update(
                IEnumerable<GameModel> existingGameModels,
                IEnumerable<GameModel> newGameModels)
            {
                var count = 0;
                foreach (var newGameModel in newGameModels)
                {
                    var modelForUpdate = existingGameModels
                        .SingleOrDefault(x => x.AppId == newGameModel.AppId);

                    if (modelForUpdate is not null)
                    {
                        modelForUpdate.Update(
                            name: newGameModel.Name,
                            publicationDate: newGameModel.PublicationDate,
                            previewURL: newGameModel.PreviewURL,
                            categories: newGameModel.Categories
                            );

                        var gameStatistic = newGameModel.GameStatistics.First();
                        gameStatistic.GameId = modelForUpdate.Id;

                        _dbContext.GamesStatistics
                            .Add(gameStatistic);
                        count++;
                    }
                }

                _logger.LogInformation($"На обновление помечено {count} игр");
            }
        }
    }
}