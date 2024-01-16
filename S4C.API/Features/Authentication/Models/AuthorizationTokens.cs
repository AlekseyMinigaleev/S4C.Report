namespace С4S.API.Features.Authentication.Models
{
    /// <summary>
    /// бъект для хранения токенов авторизации.
    /// </summary>
    public class AuthorizationTokens
    {
        /// <summary>
        /// Токен доступа
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
