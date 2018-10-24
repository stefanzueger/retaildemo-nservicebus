using System.Threading.Tasks;
using Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Shipping
{
    class ShipOrderHandler : IHandleMessages<ShipOrder>
    {
        static ILog log = LogManager.GetLogger<ShipOrderHandler>();

        public Task Handle(ShipOrder message, IMessageHandlerContext context)
        {
            log.Info($"Order [{message.OrderId}] - Succesfully shipped.");
            return Task.CompletedTask;
        }
    }
}