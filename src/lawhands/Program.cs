using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace lawhands
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT")
                .Build();
            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel(options =>
                {
                    options.UseHttps("testCert.pfx", "testPassword");
                    
                })
                .UseSetting("ASPNETCORE_ENVIRONMENT","Development")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("https://localhost:5000/")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
