using System.Linq;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MyApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IMetricsRoot Metrics { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks();

            services.AddOpenTracing(x =>
            {
                x.ConfigureAspNetCore(options =>
                {
                    options.Hosting.IgnorePatterns.Add(ctx => ctx.IsHealthCheckEndpoint());
                    options.Hosting.IgnorePatterns.Add(ctx => ctx.IsMetricsEndpoint());
                });
            });
            services.AddJaeger(Configuration);

            Metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            services.AddMetrics(Metrics);
            services.AddMetricsEndpoints(x =>
            {
                x.MetricsEndpointEnabled = false;
                x.EnvironmentInfoEndpointEnabled = false;

                x.MetricsTextEndpointOutputFormatter =
                    Metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSerilogRequestLogging(x =>
            {
                x.GetLevel = LoggingHelper.GetRequestLogLevel;
            });

            app.UseMetricsAllEndpoints();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(Constants.HealthCheckPath);
            });
        }
    }
}
