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
        public Guid Id { get; set; }

        /*TODO*/

        #region Эти поля пока не используем, тк не понятно в каком виде это должно быть

        public string? Login { get; set; }
        public string? Password { get; set; }

        #endregion Эти поля пока не используем, тк не понятно в каком виде это должно быть

        /// <summary>
        /// Ссылка на страницу разработчика
        /// </summary>
        public string DeveloperPageUrl { get; set; }

        /// <summary>
        /// Связь с <see cref="UserModel"/>
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// FK <see cref="UserModel"/>
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Список игр
        /// </summary>
        public ISet<GameModel>? Games { get; set; }

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