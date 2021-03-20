using System;
using Newtonsoft.Json;

namespace Nuka.Core.Messaging
{
    public abstract record IntegrationEvent
    {
        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }

        protected IntegrationEvent(string correlationId) : this()
        {
            CorrelationId = correlationId;
        }
        
        [JsonProperty] public string CorrelationId { get; set; }

        [JsonProperty] public Guid Id { get; private init; }

        [JsonProperty] public DateTime CreationDate { get; private init; }
    }
}