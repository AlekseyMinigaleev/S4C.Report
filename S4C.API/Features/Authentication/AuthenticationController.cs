using C4S.Helpers.ApiHeplers.Controllers;
using FluentValidation;
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
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login(
            [FromBody] Login.Query query,
            [FromServices] IValidator<Login.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);  

            var result = await Mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("createAccount")]
        public async Task<ActionResult> CreateAccount(
            [FromBody]CreateAccount.Query query,
            [FromServices] IValidator<CreateAccount.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Mediator.Send(query, cancellationToken);

            return Ok("Аккаунт успешно создан");
        }
    }
}