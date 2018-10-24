using NServiceBus;

namespace Messages.Commands
{
    public class ShipOrder : ICommand
    {
        public string OrderId { get; set; }
    }
}