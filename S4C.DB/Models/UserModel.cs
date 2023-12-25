using C4S.DB.Models.Hangfire;
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
        public string? RsyaAuthorizationToken { get; private set; }

        /// <summary>
        /// Список игр
        /// </summary>
        public ISet<GameModel> Games { get; private set; }

        /// <summary>
        /// Список конфигураций джоб
        /// </summary>
        public ISet<HangfireJobConfigurationModel> HangfireJobConfigurationModels { get; private set; }

        public UserModel(
            string login,
            string password,
            string developerPageUrl,
            ISet<GameModel> games,
            string? rsyaAuthorizationToken = default)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = developerPageUrl;
            RsyaAuthorizationToken = rsyaAuthorizationToken;
            Games = games;
            Login = login;
            Password = password;
        }

        private UserModel()
        { }

        public void SetAuthorizationToken(string rsyaAuthorizationToken)
        {
            RsyaAuthorizationToken = rsyaAuthorizationToken;
        }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для <see cref="UserModel"/>
    /// </summary>
    public static class UserExpressions
    { }
}