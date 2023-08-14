using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace C4S.ApiHelpers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected async Task ValidateAndChangeModelState<T>(IValidator<T> validator, T instance)
        {
            var validationResult = await validator.ValidateAsync(instance);
            if (!validationResult.IsValid)
                foreach (var item in validationResult.Errors)
                    ModelState.AddModelError(item.ErrorCode, item.ErrorMessage);
        }
    }
}
