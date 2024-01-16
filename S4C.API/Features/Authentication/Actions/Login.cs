using C4S.DB;
using C4S.DB.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using С4S.API.Features.Authentication.Models;
using С4S.API.Features.Authentication.ViewModels;
using С4S.API.SettingsModels;

namespace С4S.API.Features.Authentication.Actions
{
    public class Login
    {
        public class Query : IRequest<AuthorizationTokens>
        {
            /// <inheritdoc cref="UserCredentionals"/>
            public UserCredentionals UserCreditionals { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(ReportDbContext dbContext)
            {
                RuleFor(x => x.UserCreditionals)
                .Cascade(CascadeMode.Stop)
                .SetValidator(new UserCredentionalsValidator())
                .MustAsync(async (query, cancellationToken) =>
                {
                    var user = await dbContext.Users
                        .SingleOrDefaultAsync(
                            x => x.Login.Equals(query.Login) && x.Password.Equals(query.Password),
                            cancellationToken);

                    return user is not null;
                })
                .WithErrorCode(HttpStatusCode.NotFound.ToString())
                .WithMessage("Введены неверные логин или пароль");
            }
        }

        private class Handler : IRequestHandler<Query, AuthorizationTokens>
        {
            private readonly JWTConfiguration _jwt;
            private readonly ReportDbContext _dbContext;

            public Handler(
                ReportDbContext dbContext,
                IOptions<JWTConfiguration> jwt)
            {
                _jwt = jwt.Value;
                _dbContext = dbContext;
            }

            public async Task<AuthorizationTokens> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .SingleAsync(
                        x => x.Login.Equals(query.UserCreditionals.Login),
                        cancellationToken);

                var claimsPrincipal = CreateClaimsPrincipal(user);

                var response = new AuthorizationTokens
                {
                    AccessToken = _jwt.CreateJwtToken(claimsPrincipal.Claims),
                    RefreshToken = CreateRefreshToken(),
                };

                user.SetRefreshToken(
                    response.RefreshToken,
                    DateTime.Now.AddMinutes(_jwt.DurationInMinutes));

                await _dbContext.SaveChangesAsync(cancellationToken);

                return response;
            }

            private static ClaimsPrincipal CreateClaimsPrincipal(UserModel user)
            {
                var claims = CreateClaims(user);
                var claimsIdentity = new ClaimsIdentity(claims, JWTConfiguration.AuthenticationType);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                return claimsPrincipal;
            }

            private static List<Claim> CreateClaims(UserModel user)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Login),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };

                if (user.RsyaAuthorizationToken is not null)
                    claims.Add(
                        new Claim(nameof(user.RsyaAuthorizationToken),
                        user.RsyaAuthorizationToken));

                return claims;
            }

            private string CreateRefreshToken()
            {
                var randomNumbers = new byte[64];

                using var generator = RandomNumberGenerator.Create();

                generator.GetBytes(randomNumbers);

                var result = Convert.ToBase64String(randomNumbers);

                return result;
            }
        }
    }
}