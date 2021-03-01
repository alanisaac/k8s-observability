using System;
using Microsoft.AspNetCore.Http;
using Serilog.Events;

namespace MyApp
{
    public class LoggingHelper
    {
        public static LogEventLevel GetRequestLogLevel(HttpContext ctx, double _, Exception ex)
        {
            // log errors for exceptions and 500-level responses
            if (ex != null || ctx.Response.StatusCode >= 500)
            {
                return LogEventLevel.Error;
            }

            // set health checks and metrics to verbose
            if (ctx.IsHealthCheckEndpoint() || ctx.IsMetricsEndpoint())
            {
                return LogEventLevel.Verbose;
            }

            return LogEventLevel.Information;
        }
    }
}
