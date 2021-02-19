using System.Threading.Tasks;

namespace Nuka.Core.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync(IntegrationEvent integrationEvent);
    }
}