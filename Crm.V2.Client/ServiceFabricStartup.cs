using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Crm.V2.Client
{
    public class ServiceFabricStartup
    {
        public ServiceFabricStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var orleansClient = CreateOrleansClientSf();
            services.AddSingleton(orleansClient);

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "Ogun API", Version = "v1" }); });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            //else
            //    app.UseHsts();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ogun API V1"); });
            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        private IClusterClient CreateOrleansClientSf()
        {
            var serviceName = new Uri("fabric:/Ogun.SfHost/Ogun.SfHost.Service");
            var clientBuilder = new ClientBuilder()
                .UseAzureStorageClustering(options => options.ConnectionString = "UseDevelopmentStorage=true")
                .Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = serviceName.ToString();
                    options.ClusterId = "dev";
                })
                .ConfigureLogging(logging => logging.AddDebug());
            var client = clientBuilder.Build();

            client.Connect(async exception =>
            {
                Console.WriteLine(exception);
                Console.WriteLine("retrying....");
                await Task.Delay(2000);
                return true;
            }).Wait();

            return client;
        }
    }
}