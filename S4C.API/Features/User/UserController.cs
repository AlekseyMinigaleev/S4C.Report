using C4S.Helpers.ApiHeplers.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using С4S.API.Features.User.Action;


namespace С4S.API.Features.User
{
    public class UserController : BaseApiController
    {
        public UserController(IMediator mediator) : base(mediator)
        { }

        [SwaggerOperation(Summary = "https://oauth.yandex.ru/authorize?response_type=token&client_id=0110c259e959451a8633d20e21540c06")]
        [HttpPost("SetRsyaAuthorizationToken")]
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
