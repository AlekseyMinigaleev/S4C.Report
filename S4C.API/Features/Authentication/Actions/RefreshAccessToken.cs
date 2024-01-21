using C4S.DB;
using C4S.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using С4S.API.Features.Authentication.Models;

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
            private readonly IJwtService _jwtService;

            public Handler(
                ReportDbContext dbContext,
                IJwtService jwtService)
            {
                _dbContext = dbContext;
                _jwtService = jwtService;
            }

            public async Task<AuthorizationTokens?> Handle(Command request, CancellationToken cancellationToken)
            {
                var accessToken = request.AuthorizationTokens.AccessToken;
                var refreshToken = request.AuthorizationTokens.RefreshToken;

                var validation = new TokenValidationParameters()
                {
                    ValidIssuer = _jwtService.JwtConfig.Issuer,
                    ValidAudience = _jwtService.JwtConfig.Audience,
                    IssuerSigningKey = _jwtService.SymmetricSecurityKey,
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

                accessToken = _jwtService.CreateJwtToken(user, _jwtService.AccessTokenExpiry);

                return new AuthorizationTokens
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
        }
    }
}