using System;
using Jaeger;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenTracing;

namespace MyApp
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJaeger(this IServiceCollection serviceCollection, string serviceName)
        {
            serviceCollection.AddSingleton<ITracer>(x => 
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();

                Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
	                .RegisterSenderFactory<ThriftSenderFactory>();

                var tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(new ConstSampler(true))
                    .Build();
                    
                return tracer;
            });
        }
    }
}
