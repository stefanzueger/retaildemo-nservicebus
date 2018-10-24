using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Billing
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "Billing";

            var config = new EndpointConfiguration("RetailDemo.Billing");
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

            var endpointInstance = await Endpoint.Start(config)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
