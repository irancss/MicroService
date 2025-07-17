using MassTransit;

namespace BuildingBlocks.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
        Task SendAsync<T>(T message, Uri? destinationAddress = null, CancellationToken cancellationToken = default) where T : class;
    }

    public class MessageBus : IMessageBus
    {
        private readonly IBus _bus;

        public MessageBus(IBus bus)
        {
            _bus = bus;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await _bus.Publish(message, cancellationToken);
        }

        public async Task SendAsync<T>(T message, Uri? destinationAddress = null, CancellationToken cancellationToken = default) where T : class
        {
            if (destinationAddress != null)
            {
                var endpoint = await _bus.GetSendEndpoint(destinationAddress);
                await endpoint.Send(message, cancellationToken);
            }
            else
            {
                await _bus.Send(message, cancellationToken);
            }
        }
    }
}
