using C4S.Helpers.ApiHeplers.Controllers;
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
            CancellationToken cancellationToken)
        {
            var isSuccess = await Mediator.Send(command, cancellationToken);

            return isSuccess
                ? Ok("Токен авторизации успешно установлен")
                : BadRequest("Указан некорректный токен авторизации");
        }
    }
}