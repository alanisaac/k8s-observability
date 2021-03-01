using Jaeger;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJaeger(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton(x => 
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();

                Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
	                .RegisterSenderFactory<ThriftSenderFactory>();

                var config = Configuration.FromIConfiguration(loggerFactory, configuration);
                var tracer = config.GetTracer();
                return tracer;
            });
        }
    }
}