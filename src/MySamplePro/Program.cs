using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace MySamplePro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // 4. 使用 spring cloud configuration 
                //.AddConfigServer()  
                // 7. 使用apollo 配置中心
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    builder.AddApollo(builder.Build().GetSection("apollo"))
                        .AddDefault()
                        .AddNamespace("ClientService");

                })
                .UseStartup<Startup>();
    }
}
