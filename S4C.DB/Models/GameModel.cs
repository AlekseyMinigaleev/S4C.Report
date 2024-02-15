namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица игры
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; set; }

        /// <remarks>
        /// appId со страницы разработчика
        /// </remarks>
        public int AppId { get; private set; }

        /// <summary>
        /// Id страницы игры.
        /// </summary>
        /// <remarks>
        /// Поле необходимое для  получения данных с РСЯ
        /// </remarks>
        public int? PageId { get; private set; }

        /// <summary>
        /// Название игры
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime? PublicationDate { get; private set; }

        /// <summary>
        /// Пользователь, которому принадлежит игра
        /// </summary>
        public UserModel User { get; private set; }

        /// <summary>
        /// FK <see cref="UserModel"/>
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Ссылка на картинку игры
        /// </summary>
        public string? PreviewURL { get; set; }

        /// <summary>
        /// Ссылка на игру
        /// </summary>
        public string? URL => $"{User.DeveloperPageUrl}#app={AppId}";

        /// <summary>
        /// Список записей статистики
        /// </summary>
        public ISet<GameStatisticModel> GameStatistics { get; private set; }

        /// <summary>
        /// Список категорий игры
        /// </summary>
        public ISet<CategoryModel> Categories => CategoryGameModels
            .Select(x => x.Category)
            .ToHashSet();

        /// <summary>
        /// Список связей <see cref="GameModel"/> - <see cref="CategoryModel"/>
        /// </summary>
        public ISet<CategoryGameModel> CategoryGameModels
        {
            get => _categoryGameModel ?? new HashSet<CategoryGameModel>();
            set { _categoryGameModel = value; }
        }

        private ISet<CategoryGameModel>? _categoryGameModel;

        private GameModel()
        { }

        public GameModel(
            int appId,
            UserModel user,
            ISet<CategoryModel> categories,
            string? name = default,
            DateTime? publicationDate = default,
            ISet<GameStatisticModel>? gameStatistics = default)
        {
            Id = Guid.NewGuid();
            AppId = appId;
            User = user;
            UserId = user.Id;
            Name = name;
            AddCategories(categories);
            PublicationDate = publicationDate;
            GameStatistics = gameStatistics;
        }

        /// <summary>
        /// Изменяет список статусов игры на <paramref name="categories"/>
        /// </summary>
        /// <param name="categories">Новый список статусов игры</param>
        public void AddCategories(ISet<CategoryModel> categories)
        {
            CategoryGameModels.Clear();
            foreach (var category in categories)
                AddCategory(category);
        }

        /// <summary>
        /// Добавляет <paramref name="category"/> к списку статусов игры
        /// </summary>
        /// <param name="category">Добавляемый статус</param>
        public void AddCategory(CategoryModel category) =>
            CategoryGameModels.Add(new CategoryGameModel(this, category));

        /// <summary>
        /// Выполняет обновление сущности
        /// </summary>
        /// <param name="name">Название игры</param>
        /// <param name="publicationDate">дата публикации</param>
        /// <param name="previewURL">Ссылка на превью</param>
        /// <param name="categories">категории игры</param>
        public void Update(
            string name,
            DateTime publicationDate,
            string previewURL,
            ISet<CategoryModel> categories)
        {
            Name = name;
            PublicationDate = publicationDate;
            PreviewURL = previewURL;
            AddCategories(categories);
        }

        /// <summary>
        /// Устанавливает указанный <paramref name="pageId"/>
        /// </summary>
        /// <param name="pageId">Id страницы, необходимо для РСЯ</param>
        public void SetPageId(int pageId) => PageId = pageId;

        /// <summary>
        /// Проверяет есть ли изменения у модели по сравнению с <paramref name="incomingGameModel"/>
        /// </summary>
        /// <param name="incomingGameModel">Игра с которой происходит сравнение</param>
        /// <returns>
        /// <see langword="true"/> если в модели есть изменения, иначе <see langword="false"/>
        /// </returns>
        public bool HasChanges(GameModel incomingGameModel)
        {
            var hasChanges = Name != incomingGameModel.Name
                || PublicationDate != incomingGameModel.PublicationDate
                || PreviewURL != incomingGameModel.PreviewURL;

            return hasChanges;
        }
    }
}