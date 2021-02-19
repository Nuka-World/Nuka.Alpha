using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nuka.Core.TypeFinders;

namespace Nuka.Core.Messaging.ServiceBus
{
    public class ServiceBusEventHandlerHostService : BackgroundService, IAsyncDisposable
    {
        //private Dictionary<Type, IEnumerable<Type>> _handlers;
        private readonly ITypeFinder _typeFinder;
        private readonly ILogger<ServiceBusEventHandlerHostService> _logger;
        private readonly ServiceBusProcessor _processor;

        public ServiceBusEventHandlerHostService(
            string connectString,
            string topicName,
            string subscriptionName,
            ITypeFinder typeFinder,
            ILogger<ServiceBusEventHandlerHostService> logger)
        {
            _processor = new ServiceBusClient(connectString).CreateProcessor(topicName, subscriptionName);
            _typeFinder = typeFinder;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var eventHandlerTypes = _typeFinder.FindClassesOfType(typeof(IIntegrationEventHandler<>));
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync +=
                args =>
                {
                    _logger.LogInformation($"Event Handler: {args.Message.Body.ToString()}");
                    return Task.CompletedTask;
                };

            _processor.ProcessErrorAsync +=
                args =>
                {
                    _logger.LogInformation($"Event Handler Error: {args.Exception.Message}");
                    return Task.CompletedTask;
                };

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("ServiceBus EventHandler Service Started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            _logger.LogInformation("ServiceBus EventHandler Service Stop.");

            await base.StopAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _processor?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}