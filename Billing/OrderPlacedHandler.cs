using System.Threading.Tasks;
using Messages;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Billing
{
    public class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        static ILog log = LogManager.GetLogger<OrderPlacedHandler>();

        public Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.OrderId} - Charging credit card...");

            return context.Publish(new OrderBilled
            {
                OrderId = message.OrderId
            });
        }
    }
}