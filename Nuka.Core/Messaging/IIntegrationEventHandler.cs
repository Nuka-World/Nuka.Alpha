using System.Threading.Tasks;

namespace Nuka.Core.Messaging
{
    public interface IIntegrationEventHandler<in T> where T : IntegrationEvent
    {
        Task HandleAsync(T integrationEvent);
    }
}