using C4S.Services.Interfaces;

namespace С4S.API.Helpers
{
    public class RequestDelegates
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
