using System;
using System.Net;
using System.Threading.Tasks;
using Crm.V2.Implementations;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using static System.Console;

namespace Console.Host
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try
            {
                var host = await StartSilo();
                WriteLine("Hit enter to end...");
                ReadLine();

                await host.StopAsync();
                return 0;
            }
            catch (Exception e)
            {
                WriteLine(e);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "crm";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(AccountGrain).Assembly).WithReferences();
                    parts.AddFromApplicationBaseDirectory();
                })
                .AddAzureTableGrainStorage("GloballySharedAzureAccount",
                    options => options.ConnectionString = "UseDevelopmentStorage=true")
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}