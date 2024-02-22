using C4S.DB.Models;
using C4S.Shared.Models;
using Microsoft.IdentityModel.Tokens;

namespace C4S.Services.Services.JWTService
{

    /*TODO: Это модель а не сервис*/
    /// <summary>
    /// 
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Дата истечения срока жизни токена доступа
        /// </summary>
        public DateTime AccessTokenExpiry { get; }

        /// <summary>
        /// Дата истечения срока жизни токена обновления
        /// </summary>
        public DateTime RefreshTokenExpiry { get; }

        /// <summary>
        /// Создает объект типа <see cref="SecurityKey"/>, по установленному <see cref="SecurityKey"/>
        /// </summary>
        public SecurityKey SymmetricSecurityKey { get; }

        /// <summary>
        /// 
        /// </summary>
        public JwtConfiguration JwtConfig { get; }

        /// <summary>
        /// Создает JWT-токен на основе предоставленных утверждений (claims).
        /// </summary>
        /// <returns>Строка, представляющая собой JWT-токен.</returns>
        public string CreateJwtToken(UserModel user, DateTime expiry);
    }
}