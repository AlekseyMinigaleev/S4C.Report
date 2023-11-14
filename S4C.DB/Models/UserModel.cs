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

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }

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
            string login,
            string password,
            string developerPageUrl,
            string? authorizationToken = default,
            ISet<GameModel>? games = default)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = developerPageUrl;
            AuthorizationToken = authorizationToken;
            Games = games;
            Login = login;
            Password = password;
        }

        private UserModel()
        { }

        public void SetAuthorizationToken(string authorizationToken)
        {
            AuthorizationToken = authorizationToken;
        }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для <see cref="UserModel"/>
    /// </summary>
    public static class UserExpressions
    { }
}