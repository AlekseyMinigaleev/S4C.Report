﻿using C4S.ApiHelpers.Controllers;
using Microsoft.AspNetCore.Mvc;
using C4S.API.Features.Jobs.Actions;

namespace C4S.API.Features.Jobs
{
    public class JobController : BaseApiController
    {
        [HttpGet("GetAllJobs")]
        public async Task<ActionResult<GetJobs.ResponseViewModel[]>> GetJobs()
        {
            var result = await Mediator.Send(new GetJobs.Query());

            var response = result.Length == 0
                ? (ActionResult<GetJobs.ResponseViewModel[]>)NoContent()
                : (ActionResult<GetJobs.ResponseViewModel[]>)Ok(result);

            return response;
        }

        [HttpPost("UpdateAllJobs")]
        public async Task<ActionResult<UpdateJobs.ResponseViewModel>> UpdateJobs(UpdateJobs.Command request)
        {
            var result = await Mediator.Send(request);

            return Ok(result);
        }
    }
}