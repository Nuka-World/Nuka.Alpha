using Nuka.Core.Messaging;

namespace Nuka.Sample.API.Messaging
{
    public record SampleEvent : IntegrationEvent
    {
        public string ItemId;
    }
}