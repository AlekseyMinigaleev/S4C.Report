using System.Linq.Expressions;

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
        /// <remarks>
        /// Является дублированием appId со страницы разработчика
        /// </remarks>
        public int Id { get; private set; }

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
            int id,
            UserModel user,
            string? name = default,
            DateTime? publicationDate = default,
            ISet<GameStatisticModel>? gameStatistics = default)
        {
            Id = id;
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
        /// Проверяет есть ли изменения у модели по сравнению с <paramref name="incomingGame"/>
        /// </summary>
        /// <param name="incomingGame">Игра с которой происходит сравнение</param>
        /// <returns>
        /// <see langword="true"/> если в модели есть изменения, иначе <see langword="false"/>
        /// </returns>
        public bool HasChanges(GameModel incomingGame)
        {
            var hasChanges = Name == incomingGame.Name
                && PublicationDate == incomingGame.PublicationDate;

            return hasChanges;
        }
    }

    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameModel"/>
    /// </summary>
    public static class GameExpressions
    { }
}