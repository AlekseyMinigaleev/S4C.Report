using C4S.Helpers.ApiHeplers.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using С4S.API.Features.Authentication.Actions;

namespace С4S.API.Features.Authentication
{
    public class AuthenticationController : BaseApiController
    {
        public AuthenticationController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Осуществляет аутентификацию пользователя
        /// </summary>
        /// <returns>
        /// Jwt-токен пользователя
        /// </returns>
        [HttpPost("Login")]
        public async Task<ActionResult> Login(
            [FromBody] Login.Query query,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(query, cancellationToken);

            return result is null
                ? BadRequest("Введены неверные логин или пароль")
                : Ok(result);
        }

        [Authorize]
        [HttpPost("SignUp")]
        public async Task<ActionResult> SignUp(
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}