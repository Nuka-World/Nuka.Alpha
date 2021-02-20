using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nuka.Core.TypeFinders;

namespace Nuka.Core.Messaging.ServiceBus
{
    public class ServiceBusEventHandlerHostService : BackgroundService, IAsyncDisposable
    {
        private readonly ITypeFinder _typeFinder;
        private readonly ILifetimeScope _autofac;
        private readonly ILogger<ServiceBusEventHandlerHostService> _logger;
        private readonly ServiceBusProcessor _processor;
        private readonly Dictionary<Type, List<Type>> _eventHandlerTypesMap;

        private readonly string AUTOFAC_SCOPE_NAME = "service_bus_event_scope";

        public ServiceBusEventHandlerHostService(
            string connectString,
            string topicName,
            string subscriptionName,
            ITypeFinder typeFinder,
            ILifetimeScope autofac,
            ILogger<ServiceBusEventHandlerHostService> logger)
        {
            _logger = logger;
            _typeFinder = typeFinder;
            _autofac = autofac;

            _eventHandlerTypesMap = new Dictionary<Type, List<Type>>();
            _processor = new ServiceBusClient(connectString).CreateProcessor(topicName, subscriptionName);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var eventHandlerTypes = _typeFinder.FindClassesOfType(typeof(IIntegrationEventHandler<>));

            foreach (var eventHandlerType in eventHandlerTypes)
            {
                var eventType = eventHandlerType
                    .FindInterfaces((_, _) => true, typeof(IIntegrationEventHandler<>))
                    .First()
                    .GetGenericArguments()
                    .First();

                if (_eventHandlerTypesMap.ContainsKey(eventType))
                    _eventHandlerTypesMap[eventType].Add(eventHandlerType);
                else
                    _eventHandlerTypesMap[eventType] = new List<Type> {eventHandlerType};
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("ServiceBus EventHandler Service Started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            _logger.LogInformation("ServiceBus EventHandler Service Stop.");

            await base.StopAsync(cancellationToken);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var eventTypeName = args.Message.ApplicationProperties["event-type"].ToString();
            var (eventType, eventHandlerTypes) =
                _eventHandlerTypesMap.FirstOrDefault(mapper => mapper.Key.FullName == eventTypeName);

            if (eventHandlerTypes != null && eventHandlerTypes.Count > 0)
            {
                await using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var eventHandler = scope.ResolveOptional(eventHandlerType);
                    if (eventHandler == null) continue;

                    var integrationEvent = JsonConvert.DeserializeObject(args.Message.Body.ToString(), eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    ((Task) concreteType.GetMethod("HandleAsync")
                        ?.Invoke(eventHandler, new object[] {integrationEvent}))?.GetAwaiter().GetResult();
                }
            }

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogInformation($"Event Handler Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return _processor?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}