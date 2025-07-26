using MassTransit;

namespace BuildingBlocks.Messaging
{
    /// <summary>
    /// [اصلاح شد] این اینترفیس اکنون به وضوح برای ارسال Commands به یک صف مشخص (Send) استفاده می‌شود.
    /// برای انتشار Events باید از IEventBus استفاده کرد.
    /// </summary>
    public interface IMessageBus
    {
        Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
        Task SendAsync<T>(T message, Uri destinationAddress, CancellationToken cancellationToken = default) where T : class;
    }

    public class MessageBus : IMessageBus
    {
        private readonly IBus _bus;

        public MessageBus(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            // MassTransit attempts to send to a default endpoint for the message type
            await _bus.Send(message, cancellationToken);
        }

        public async Task SendAsync<T>(T message, Uri destinationAddress, CancellationToken cancellationToken = default) where T : class
        {
            var endpoint = await _bus.GetSendEndpoint(destinationAddress);
            await endpoint.Send(message, cancellationToken);
        }
    }
}