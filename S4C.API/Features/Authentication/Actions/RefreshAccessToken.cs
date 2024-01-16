using C4S.DB;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using С4S.API.Features.Authentication.Models;
using С4S.API.SettingsModels;

namespace С4S.API.Features.Authentication.Actions
{
    public class RefreshAccessToken
    {
        public class Command : IRequest<AuthorizationTokens>
        {
            public AuthorizationTokens AuthorizationTokens { get; set; }
        }

        public class Handler : IRequestHandler<Command, AuthorizationTokens>
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

            public async Task<AuthorizationTokens?> Handle(Command request, CancellationToken cancellationToken)
            {
                var accessToken = request.AuthorizationTokens.AccessToken;
                var refreshToken = request.AuthorizationTokens.RefreshToken;

                var validation = new TokenValidationParameters()
                {
                    ValidIssuer = _jwt.Issuer,
                    ValidAudience = _jwt.Audience,
                    IssuerSigningKey = _jwt.GetSymmetricSecurityKey(),
                    ValidateLifetime = false,
                };

                var principal = new JwtSecurityTokenHandler().
                    ValidateToken(accessToken, validation, out _);

                if (principal?.Identity?.Name is null)
                    return null;

                var user = await _dbContext.Users
                    .SingleOrDefaultAsync(
                        x => x.Login.Equals(principal.Identity.Name),
                        cancellationToken);

                if (user is null
                    || user.RefreshToken != refreshToken
                    || user.RefreshTokenExpiry < DateTime.Now)
                    return null;

                accessToken = _jwt.CreateJwtToken(principal.Claims);

                return new AuthorizationTokens
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
        }
    }
}