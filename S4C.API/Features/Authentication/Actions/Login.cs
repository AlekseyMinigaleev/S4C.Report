using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using С4S.API.Features.Authentication.Models;
using С4S.API.Features.Authentication.ViewModels;

namespace С4S.API.Features.Authentication.Actions
{
    public class Login
    {
        public class Query : IRequest<ResponseViewModel>
        {
            /// <inheritdoc cref="UserCredentionals"/>
            public UserCredentionals UserCreditionals { get; set; }
        }

        public class ResponseViewModel
        {
            public AuthorizationTokens AuthorizationTokens { get; set; }

            public DeveloperInfoViewModel DeveloperInfo { get; set; }
        }

        public class DeveloperInfoViewModel
        {
            public string DeveloperName { get; set; }
            public string DeveloperPageUrl { get; set; }
        }

        public class DeveloperInfoProfiler : Profile
        {
            public DeveloperInfoProfiler()
            {
                CreateMap<UserModel, DeveloperInfoViewModel>()
                    .ForMember(dest => dest.DeveloperName, opt => opt.MapFrom(src => src.GetDeveloperName()));
            }
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

        private class Handler : IRequestHandler<Query, ResponseViewModel>
        {
            private readonly IJwtService _jwtService;
            private readonly IMapper _mapper;
            private readonly ReportDbContext _dbContext;

            public Handler(
                ReportDbContext dbContext,
                IJwtService jwtService,
                IMapper mapper)
            {
                _jwtService = jwtService;
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<ResponseViewModel> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .SingleAsync(
                        x => x.Login.Equals(query.UserCreditionals.Login),
                        cancellationToken);

                var authorizationTokens =
                    await CreateAuthorizationTokensAndUpdateDBAsync(user, cancellationToken);

                var developerInfo = _mapper.Map<DeveloperInfoViewModel>(user);

                var response = new ResponseViewModel
                {
                    AuthorizationTokens = authorizationTokens,
                    DeveloperInfo = developerInfo,
                };

                return response;
            }

            private async Task<AuthorizationTokens> CreateAuthorizationTokensAndUpdateDBAsync(
                UserModel user,
                CancellationToken cancellationToken)
            {
                var authorizationTokens = new AuthorizationTokens
                {
                    AccessToken = _jwtService.CreateJwtToken(user, _jwtService.AccessTokenExpiry),
                    RefreshToken = _jwtService.CreateJwtToken(user, _jwtService.RefreshTokenExpiry),
                };

                user.SetRefreshToken(authorizationTokens.RefreshToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return authorizationTokens;
            }
        }
    }
}