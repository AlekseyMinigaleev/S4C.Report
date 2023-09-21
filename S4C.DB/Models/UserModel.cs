using System.Linq.Expressions;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица C4S пользователя
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /*TODO:*/

        #region на эти поля пока нет ограничений, поскольку не понятно в каком виде это должно быть

        public string Login { get; set; }
        public string Password { get; set; }

        #endregion на эти поля пока нет ограничений, поскольку не понятно в каком виде это должно быть

        /// <summary>
        /// Ссылка на страницу разработчика
        /// </summary>
        public string DeveloperPageUrl { get; private set; }

        /// <summary>
        /// Токен авторизации
        /// </summary>
        /// <remarks>
        /// РСЯ
        /// </remarks>
        public string? AuthorizationToken { get; private set; }

        /// <summary>
        /// Список игр
        /// </summary>
        public ISet<GameModel>? Games { get; private set; }

        public UserModel(
            string developerPageUrl,
            string? authorizationToken = default,
            ISet<GameModel>? games = default)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = developerPageUrl;
            AuthorizationToken = authorizationToken;
            Games = games;
        }

        private UserModel()
        { }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для <see cref="UserModel"/>
    /// </summary>
    public static class UserExpressions
    { }
}