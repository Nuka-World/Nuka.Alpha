using Nuka.Core.Messaging;

namespace Nuka.Sample.API.Messaging.EventPublish
{
    public record SampleEvent : IntegrationEvent
    {
        public string ItemId;
    }
}