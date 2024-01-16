using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        /// Тип аутентификации
        /// </summary>
        public static readonly string AuthenticationType = "JWT";

        /// <summary>
        /// Создает объект типа <see cref="Microsoft.IdentityModel.Tokens.SecurityKey"/>, по установленному <see cref="SecurityKey"/>
        /// </summary>
        public SecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

        /// <summary>
        /// Создает JWT-токен на основе предоставленных утверждений (claims).
        /// </summary>
        /// <returns>Строка, представляющая собой JWT-токен.</returns>
        public string CreateJwtToken(IEnumerable<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                    issuer: Issuer,
                    audience: Audience,
                    expires: DateTime.Now.Add(TimeSpan.FromSeconds(30)),
                    signingCredentials: new SigningCredentials(
                        GetSymmetricSecurityKey(),
                        SecurityAlgorithms.HmacSha256),
                    claims: claims);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}