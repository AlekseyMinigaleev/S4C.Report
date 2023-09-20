using System.Linq.Expressions;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица аккаунта Яндекс игр
    /// </summary>
    public class YandexGamesAccountModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Ссылка на страницу разработчика
        /// </summary>
        public string DeveloperPageUrl { get; private set; }

        /// <summary>
        /// Токен авторизации
        /// </summary>
        /// <remarks>
        /// partner2.yandex.ru/api/statistics2
        /// </remarks>
        public string AuthorizationToken { get; private set; }

        /// <summary>
        /// Связь с <see cref="UserModel"/>
        /// </summary>
        public UserModel User { get; private set; }

        /// <summary>
        /// FK <see cref="UserModel"/>
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Список игр
        /// </summary>
        public ISet<GameModel>? Games { get; private set; }

        public YandexGamesAccountModel(
            string developerPageUrl,
            UserModel user,
            ISet<GameModel>? games = default)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = developerPageUrl;
            User = user;
            UserId = user.Id;
            Games = games;
        }

        private YandexGamesAccountModel()
        { }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для модели <see cref="YandexGamesUserExpression"/>/>
    /// </summary>
    public class YandexGamesUserExpression
    { }
}