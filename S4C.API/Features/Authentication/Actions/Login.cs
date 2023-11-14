using C4S.DB;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using С4S.API.Features.Authentication.ViewModels;
using С4S.API.SettingsModels;

namespace С4S.API.Features.Authentication.Actions
{
    public class Login
    {
        public class Query : IRequest<string?>
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

        private class Handler : IRequestHandler<Query, string?>
        {
            private readonly JWTConfiguration _jwt;

            public Handler(
                IOptions<JWTConfiguration> jwt)
            {
                _jwt = jwt.Value;
            }

            public async Task<string?> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var claimsPrincipal = CreateClaims(query.UserCreditionals.Login);
                var jwtToken = CreateJwtToken(claimsPrincipal.Claims);
                return jwtToken;
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