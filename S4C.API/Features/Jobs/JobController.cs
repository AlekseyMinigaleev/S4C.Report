using C4S.API.Features.Jobs.Actions;
using C4S.ApiHelpers.Controllers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace C4S.API.Features.Jobs
{
    public class JobController : BaseApiController
    {
        public JobController(
            IMediator mediator)
            : base(mediator)
        { }

        [HttpGet("GetAllJobs")]
        public async Task<ActionResult<GetJobs.ResponseViewModel[]>> GetJobsAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetJobs.Query(), cancellationToken);

            return Ok(result);
        }

        [HttpPost("UpdateAllJobs")]
        public async Task<ActionResult<UpdateJobs.ResponseViewModel>> UpdateJobsAsync(
            UpdateJobs.Command request,
            IValidator<UpdateJobs.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, request, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}