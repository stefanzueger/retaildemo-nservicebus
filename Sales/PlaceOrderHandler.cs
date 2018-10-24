using System;
using System.Threading.Tasks;
using Messages;
using Messages.Commands;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Sales
{
    public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
    {
        private static readonly ILog Log = LogManager.GetLogger<PlaceOrderHandler>();

        static Random random = new Random();

        public Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            Log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");

            if (random.Next(0, 5) == 0)
            {
                throw new Exception("Oops");
            }

            var orderPlaced = new OrderPlaced
            {
                OrderId = message.OrderId
            };
            return context.Publish(orderPlaced);
        }
    }
}