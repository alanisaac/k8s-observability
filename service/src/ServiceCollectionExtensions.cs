using Jaeger;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJaeger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(x => 
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();

                Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
	                .RegisterSenderFactory<ThriftSenderFactory>();

                var config = Configuration.FromEnv(loggerFactory);
                var tracer = config.GetTracer();
                return tracer;
            });
        }
    }
}