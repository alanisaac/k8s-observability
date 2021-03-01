using System.Collections.Generic;
using System.Linq;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyApp
{
    public class Startup
    {
        private const string _healthCheckPath = "/ping";
        private readonly HashSet<string> _metricsPaths = new HashSet<string>
        {
            "/metrics",
            "/metrics-text",
            "/env"
        };

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
                    options.Hosting.IgnorePatterns.Add(ctx => ctx.Request.Path == _healthCheckPath);
                    options.Hosting.IgnorePatterns.Add(ctx => _metricsPaths.Contains(ctx.Request.Path));
                });
            });
            services.AddJaeger(Configuration);

            Metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Build();

            services.AddMetrics(Metrics);
            services.AddMetricsEndpoints(x =>
            {
                x.MetricsTextEndpointOutputFormatter =
                    Metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                x.MetricsEndpointOutputFormatter =
                    Metrics.OutputMetricsFormatters.OfType<MetricsPrometheusProtobufOutputFormatter>().First();
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

            app.UseMetricsAllEndpoints();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(_healthCheckPath);
            });
        }
    }
}
