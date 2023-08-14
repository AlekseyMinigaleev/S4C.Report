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
    }
}
