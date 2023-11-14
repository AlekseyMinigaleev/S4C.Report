using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace С4S.API.SettingsModels
{
    /// <summary>
    /// Конфигурация JWT токена
    /// </summary>
    public class JWTConfiguration
    {
        /// <summary>
        /// Секретный ключ
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Издатель
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Аудитория для коотрой предназначен токен
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Срок жизни токена
        /// </summary>
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// Создает объект типа <see cref="Microsoft.IdentityModel.Tokens.SecurityKey"/>, по установленному <see cref="SecurityKey"/>
        /// </summary>
        public SecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

        /// <summary>
        /// Тип аутентификации
        /// </summary>
        public readonly static string AuthenticationType = "JWT";
    }
}
