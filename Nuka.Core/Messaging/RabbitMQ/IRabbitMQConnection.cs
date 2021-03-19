using System;
using RabbitMQ.Client;

namespace Nuka.Core.Messaging.RabbitMQ
{
    public interface IRabbitMQConnection: IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}