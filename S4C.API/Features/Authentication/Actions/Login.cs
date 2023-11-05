using C4S.DB;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using С4S.API.SettingsModels;

namespace С4S.API.Features.Authentication.Actions
{
    public class Login
    {
        public class Query : IRequest<string?>
        {
            /// <summary>
            /// Логин пользователя
            /// </summary>
            public string Login { get; set; }

            /// <summary>
            /// Пароль пользователя
            /// </summary>
            public string Password { get; set; }
        }

        private class Handler : IRequestHandler<Query, string?>
        {
            private readonly ReportDbContext _dbContext;
            private readonly JWTConfiguration _jwt;

            public Handler(
                ReportDbContext dbContext,
                IOptions<JWTConfiguration> jwt)
            {
                _dbContext = dbContext;
                _jwt = jwt.Value;
            }

            public async Task<string?> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var isValidQuery = await ValidateQueryAsync(query, cancellationToken);

                if (!isValidQuery)
                    return null;

                var claimsPrincipal = CreateClaims(query.Login);
                var jwtToken = CreateJwtToken(claimsPrincipal.Claims);
                return jwtToken;
            }

            private async Task<bool> ValidateQueryAsync(
                Query query,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                   .SingleOrDefaultAsync(
                       x => x.Login.Equals(query.Login) && x.Password.Equals(query.Password),
                       cancellationToken);

                return user is not null;
            }

            private static ClaimsPrincipal CreateClaims(string login)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login)
                };
                var claimsIdentity = new ClaimsIdentity(claims, JWTConfiguration.AuthenticationType);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                return claimsPrincipal;
            }

            private string CreateJwtToken(IEnumerable<Claim> claims)
            {
                var jwt = new JwtSecurityToken(
                        issuer: _jwt.Issuer,
                        audience: _jwt.Audience,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwt.DurationInMinutes)),
                        signingCredentials: new SigningCredentials(_jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256),
                        claims: claims);

                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
        }
    }
}