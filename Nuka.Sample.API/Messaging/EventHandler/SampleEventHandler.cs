﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nuka.Core.Messaging;
using Nuka.Sample.API.Messaging.EventPublish;

namespace Nuka.Sample.API.Messaging.EventHandler
{
    public class SampleEventHandler : IIntegrationEventHandler<SampleEvent>
    {
        private readonly ILogger<SampleEventHandler> _logger;

        public SampleEventHandler(ILogger<SampleEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(SampleEvent integrationEvent)
        {
            await Task.Run(() =>
            {
                var timer = new Random().Next(15);
                Thread.Sleep(timer * 1000);
                _logger.LogInformation(
                    $"Event Handler #1 Successfully. Item ID:{integrationEvent.ItemId}. Timer: {timer}");
            });
        }
    }
}