using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using Raven.Client.Document;

namespace Shipping
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "Shipping";

            var config = new EndpointConfiguration("RetailDemo.Shipping");
            config.AssemblyScanner();
            config.LimitMessageProcessingConcurrencyTo(1);
            config.SendFailedMessagesTo("RetailDemo.error");
            config.AuditProcessedMessagesTo("RetailDemo.audit");
            config.EnableInstallers();

            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "RetailDemo.Shipping"
            };
            documentStore.Initialize(ensureDatabaseExists: true);

            var persistence = config.UsePersistence<RavenDBPersistence>();
            persistence.DisableSubscriptionVersioning();
            persistence.SetDefaultDocumentStore(documentStore);

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

            documentStore.Dispose();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
