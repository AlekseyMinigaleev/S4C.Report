﻿using AngleSharp;
using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="ICategoriesSyncService"/>
    public class CategoriesSyncService : ICategoriesSyncService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IBrowsingContext _browsingContext;
        private readonly string _categoriesURL;
        private BaseLogger _logger;

        public CategoriesSyncService(
            ReportDbContext dbContext,
            IBrowsingContext browsingContext,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _dbContext = dbContext;
            _browsingContext = browsingContext;
            _categoriesURL = configuration["CategoriesURL"]!;
            ArgumentException.ThrowIfNullOrEmpty(
               _categoriesURL,
               "в файле appsetting.json не указана или указана неверно ссылка на категории");
        }

        /// <inheritdoc/>
        public async Task SyncCategoriesAsync(BaseLogger logger, CancellationToken cancellationToken)
        {
            _logger = logger;

            _logger.LogInformation("Запущен процесс синхронизации статусов игр");

            _logger.LogInformation("Заупущен процесс получения статусов");
            var incomingCategories = await GetIncomingCategories(cancellationToken);
            _logger.LogSuccess($"процесс получения статусов успешно завершен. Получено {incomingCategories.Count} статусов");

            _logger.LogInformation("Запущен процесс обработки полученных статусов");
            await ProseccIncomindgCategories(incomingCategories, cancellationToken);
            _logger.LogSuccess("процесс обработки полученных статусов успешно завершен");

            _logger.LogSuccess("процесс синхронизации статусов игр успешно завершен");
        }

        private async Task<List<CategoryModel>> GetIncomingCategories(CancellationToken cancellationToken)
        {
            var incomingCategoryModels = new List<CategoryModel>();

            var document = await _browsingContext
                .OpenAsync(_categoriesURL, cancellationToken);

            var categoriesColumnListArray = document
                .QuerySelector(".categories-page__columns")?.Children ?? throw new Exception();

            foreach (var categoryColumn in categoriesColumnListArray)
            {
                var categoryList = categoryColumn.Children;

                foreach (var category in categoryList)
                {
                    var spanElement = category.QuerySelector("span.category-wrapper") ?? throw new Exception();

                    var dataName = spanElement.GetAttribute("data-name") ?? throw new Exception();
                    var title = spanElement.GetAttribute("title") ?? throw new Exception();

                    var categoryModel = new CategoryModel(dataName, title);
                    incomingCategoryModels.Add(categoryModel);
                }
            }

            return incomingCategoryModels;
        }

        private async Task ProseccIncomindgCategories(
            IEnumerable<CategoryModel> incomingCategories,
            CancellationToken cancellationToken)
        {
            var existCategories = await _dbContext.Categories.ToListAsync(cancellationToken);

            var counOfDeletedCategories = RemoveFromDb(existCategories, incomingCategories);
            _logger.LogInformation($"Добавлено на удаление {counOfDeletedCategories} категорий ");

            var counOfAddedCategories = AddToDb(existCategories, incomingCategories);
            _logger.LogInformation($"Добавлено на добавление {counOfAddedCategories} категорий");

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"База данных обновлена");
        }

        private int RemoveFromDb(
            IEnumerable<CategoryModel> existCategories,
            IEnumerable<CategoryModel> incomingCategories)
        {
            var categoriesToDelete = existCategories
                .GetItemsNotInSecondCollection(incomingCategories);

            _dbContext.Categories.RemoveRange(categoriesToDelete);

            return categoriesToDelete.Count();
        }

        private int AddToDb(
            IEnumerable<CategoryModel> existCategories,
            IEnumerable<CategoryModel> incomingCategories)
        {
            var categoriesToAdd = incomingCategories
                .GetItemsNotInSecondCollection(existCategories);

            _dbContext.Categories.AddRange(categoriesToAdd);

            return categoriesToAdd.Count();
        }
    }
}