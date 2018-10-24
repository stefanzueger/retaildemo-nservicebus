using System;
using System.Threading.Tasks;
using Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace ClientUI
{
    class Program
    {
        static ILog log = LogManager.GetLogger<Program>();

        static async Task Main()
        {
            Console.Title = "ClientUI";

            var config = new EndpointConfiguration("RetailDemo.ClientUI");
            config.AssemblyScanner();
            config.UsePersistence<InMemoryPersistence>();
            config.LimitMessageProcessingConcurrencyTo(1);
            config.SendFailedMessagesTo("RetailDemo.error");
            config.AuditProcessedMessagesTo("RetailDemo.audit");
            config.EnableInstallers();

            var metrics = config.EnableMetrics();

            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: "Particular.Monitoring",
                interval: TimeSpan.FromSeconds(2)
            );

            var transport = config.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString("host=localhost");

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(PlaceOrder), "RetailDemo.Sales");

            var endpointInstance = await Endpoint.Start(config)
                .ConfigureAwait(false);

            await RunLoop(endpointInstance)
                .ConfigureAwait(false);

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            while (true)
            {
                log.Info("Press 'P' to place an order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the command
                        var command = new PlaceOrder
                        {
                            OrderId = Guid.NewGuid().ToString()
                        };

                        // Send the command to the local endpoint
                        log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await endpointInstance.Send(command)
                            .ConfigureAwait(false);

                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}
