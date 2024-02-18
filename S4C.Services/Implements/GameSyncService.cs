using AutoMapper;
using C4S.Common.Models;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using S4C.YandexGateway.DeveloperPage;
using S4C.YandexGateway.DeveloperPage.Models;
using S4C.YandexGateway.RSYA;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameSyncService"/>
    public class GameSyncService : IGameSyncService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IDeveloperPageParser _developerPageParser;
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly IRsyaGateway _rsyaGateway;
        private readonly IMapper _mapper;
        private BaseLogger _logger;
        private UserModel _user;
        private IQueryable<GameModel> _existingGameModelsQuery;

        public GameSyncService(
            ReportDbContext dbContext,
            IDeveloperPageParser developerPageParser,
            IDeveloperPageGetaway developerPageGetaway,
            IRsyaGateway rsyaGateway,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _developerPageParser = developerPageParser;
            _developerPageGetaway = developerPageGetaway;
            _rsyaGateway = rsyaGateway;
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
            var incomingAppIds = await _developerPageParser
                .GetAppIdsAsync(_user.DeveloperPageUrl, _logger, cancellationToken);

            var gameInfoModels = await _developerPageGetaway
                .GetGamesInfoAsync(incomingAppIds, _logger, cancellationToken);

            _logger.LogInformation("Обработка полученных данных");
            var newGameModels = new List<GameModel>();
            foreach (var gameInfoModel in gameInfoModels)
            {
                var newGameModel = _mapper.Map<GameInfoModel, GameModel>(gameInfoModel);
                var newGameStatistic = _mapper.Map<GameInfoModel, GameStatisticModel>(gameInfoModel);

                newGameModel.GameStatistics = new HashSet<GameStatisticModel>() { newGameStatistic };

                await EnrichGameModelsAsync(
                    newGameModel,
                    gameInfoModel,
                    cancellationToken);

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

        private async Task EnrichGameModelsAsync(
            GameModel newGameModel,
            GameInfoModel gameInfoModel,
            CancellationToken cancellationToken)
        {
            var authorizationToken = _user.RsyaAuthorizationToken;

            await EnrichCategories(gameInfoModel, newGameModel, cancellationToken);

            if (authorizationToken is not null)
                await EnrichCashIncomeAsync(newGameModel, cancellationToken);

            async Task EnrichCategories(
                GameInfoModel gameInfoModel,
                GameModel newGameModel,
                CancellationToken cancellationToken)
            {
                var categories = await GetCategoriesAsync(gameInfoModel, cancellationToken);
                newGameModel.AddCategories(categories);

                async Task<IEnumerable<CategoryModel>> GetCategoriesAsync(
                GameInfoModel incomingGameInfo,
                CancellationToken cancellationToken)
                {
                    var existCategories = _dbContext.Categories;

                    var incomingCategories = incomingGameInfo.CategoriesNames;

                    var categories = await existCategories
                        .Where(x => incomingCategories
                            .Contains(x.Name))
                        .ToListAsync(cancellationToken);

                    return categories;
                }
            }

            async Task EnrichCashIncomeAsync(
            GameModel newGameModel,
            CancellationToken cancellationToken)
            {
                var existGameModel = _existingGameModelsQuery
                    .Include(x => x.GameStatistics)
                    .SingleOrDefault(x => x.AppId == newGameModel.AppId);

                if (existGameModel is not null && existGameModel.PageId.HasValue)
                {
                    var startDate = newGameModel.PublicationDate;
                    var endDate = DateTime.Now;
                    var period = new DateTimeRange(startDate, endDate);

                    var cashIncome = await _rsyaGateway
                        .GetAppCashIncomeAsync(
                            pageId: existGameModel.PageId.Value,
                            authorization: authorizationToken,
                            period: period,
                            cancellationToken);

                    var lastSynchroGameStatistic = existGameModel.GameStatistics
                        .OrderByDescending(x => x.LastSynchroDate)
                        .First();

                    lastSynchroGameStatistic.SetCashIncome(cashIncome);
                }
            }
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

                gameModelsToAdd.ToList().ForEach(x => x.UserId = _user.Id);

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