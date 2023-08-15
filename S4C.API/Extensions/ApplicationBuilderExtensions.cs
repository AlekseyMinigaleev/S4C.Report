using С4S.API.Middlewares;

namespace С4S.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseStartExecutedMiddlewares(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InitHangfireJobsMiddleware>();
        }
    }
}