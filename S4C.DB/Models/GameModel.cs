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
        /// Список записей статистики
        /// </summary>
        public ISet<GameStatisticModel> GameStatistics { get; private set; }

        private GameModel()
        { }

        public GameModel(
            int appId,
            UserModel user,
            string? name = default,
            DateTime? publicationDate = default,
            ISet<GameStatisticModel>? gameStatistics = default)
        {
            Id = Guid.NewGuid();
            AppId = appId;
            User = user;
            UserId = user.Id;
            Name = name;
            PublicationDate = publicationDate;
            GameStatistics = gameStatistics;
        }

        /// <summary>
        /// Выполняет обновление сущности
        /// </summary>
        /// <param name="name">Название игры</param>
        /// <param name="publicationDate">дата публикации</param>
        public void Update(string name, DateTime publicationDate)
        {
            Name = name;
            PublicationDate = publicationDate;
        }

        /// <summary>
        /// Устанавливает указанный <paramref name="pageId"/>
        /// </summary>
        /// <param name="pageId">Id страницы, необходимо для РСЯ</param>
        public void SetPageId(int pageId) => PageId = pageId;

        /// <inheritdoc cref="GameModel.HasChanges(GameModel)"/>
        /// <param name="incomingFields"> Поля с которым происходит сравнение</param>
        public bool HasChanges(GameModifiableFields incomingFields)
        {
            var hasChanges = Name == incomingFields.Name
                && PublicationDate == incomingFields.PublicationDate;

            return hasChanges;
        }

        /// <summary>
        /// Проверяет есть ли изменения у модели по сравнению с <paramref name="incomingGameModel"/>
        /// </summary>
        /// <param name="incomingGameModel">Игра с которой происходит сравнение</param>
        /// <returns>
        /// <see langword="true"/> если в модели есть изменения, иначе <see langword="false"/>
        /// </returns>
        public bool HasChanges(GameModel incomingGameModel)
        {
            var hasChanges = Name == incomingGameModel.Name
                && PublicationDate == incomingGameModel.PublicationDate;

            return hasChanges;
        }
    }

    /// <summary>
    /// Изменяемые поля модели <see cref="GameModel"/>
    /// </summary>
    public class GameModifiableFields
    {
        /// <inheritdoc cref="GameModel.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="GameModel.PublicationDate"/>
        public DateTime PublicationDate { get; set; }

        public GameModifiableFields(
            string name,
            DateTime publicationDate)
        {
            Name = name;
            PublicationDate = publicationDate;
        }

        private GameModifiableFields()
        { }
    }
}