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
        /// Токен обновления
        /// </summary>
        public string? RefreshToken { get; private set; }

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
            string? rsyaAuthorizationToken = default,
            string? refreshToken = null)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = developerPageUrl;
            RsyaAuthorizationToken = rsyaAuthorizationToken;
            Games = games;
            Login = login;
            Password = password;
            RefreshToken = refreshToken;
        }

        private UserModel()
        { }

        /// <summary>
        /// Устанавливает токен авторизации
        /// </summary>
        public void SetAuthorizationToken(string rsyaAuthorizationToken)
        {
            RsyaAuthorizationToken = rsyaAuthorizationToken;
        }

        /// <summary>
        /// Устанавливает токен обновления
        /// </summary>
        public void SetRefreshToken(string? token)
        {
            RefreshToken = token;
        }

        /// <summary>
        /// Удаляет токен обновления
        /// </summary>
        public void ClearRefreshToken()
        {
            RefreshToken = null;
        }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для <see cref="UserModel"/>
    /// </summary>
    public static class UserExpressions
    { }
}