using C4S.Helpers.ApiHeplers.Controllers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using С4S.API.Features.User.Action;

namespace С4S.API.Features.User
{
    public class UserController : BaseApiController
    {
        public UserController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Устанавливает токен авторизации.
        /// https://yandex.ru/dev/partner-statistics/doc/ru/concepts/access
        /// </summary>
        [Authorize]
        [HttpPut("SetRsyaAuthorizationToken")]
        public async Task<ActionResult> SetRsyaAuthorizationTokenAsync(
            [FromBody] SetRsyaAuthorizationToken.Command command,
            [FromServices] IValidator<SetRsyaAuthorizationToken.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, command, cancellationToken);

            if(!ModelState.IsValid)
                return BadRequest("Указан некорректный токен авторизации");

            await Mediator.Send(command, cancellationToken);

            return Ok("Токен авторизации успешно установлен");
        }
    }
}