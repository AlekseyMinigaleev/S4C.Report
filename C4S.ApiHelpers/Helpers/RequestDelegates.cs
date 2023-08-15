
using C4S.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace C4S.ApiHelpers.Helpers
{
    public static class RequestDelegates
    {
        public static RequestDelegate InitMissingHangfireJobs =>
            async context =>
            {
                var service = context.RequestServices.GetService<IBackGroundJobService>() 
                    ?? throw new ArgumentNullException(nameof(IBackGroundJobService));
                await service.AddMissingHangfirejobs();
            };
    }
}
