using C4S.Services.Interfaces;

namespace С4S.API.Middlewares
{
    public class InitHangfireJobsMiddleware
    {
        private readonly RequestDelegate _next;

        public InitHangfireJobsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IBackGroundJobService jobService)
        {
            await jobService.AddMissingHangfirejobs();
            await _next(context);
        }   
    }
}
