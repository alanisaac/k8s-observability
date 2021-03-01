using Microsoft.AspNetCore.Http;

namespace MyApp
{
    public static class HttpContextExtensions
    {
        public static bool IsHealthCheckEndpoint(this HttpContext httpContext)
        {
            return httpContext.Request.Path == Constants.HealthCheckPath;
        }

        public static bool IsMetricsEndpoint(this HttpContext httpContext)
        {
            return httpContext.Request.Path == Constants.MetricsPath;
        }
    }
}
