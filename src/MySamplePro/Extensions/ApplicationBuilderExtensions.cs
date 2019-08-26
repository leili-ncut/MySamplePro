using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MySamplePro.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IApplicationLifetime lifetime,IServiceProvider serviceProvider)
        {
            // 获取配置文件
            var serviceProvider2 = app.ApplicationServices;
            var service = serviceProvider2.GetService<IConfiguration>();
            var address = service.GetSection("ConsulConfig:Address");

            var consulClient = new ConsulClient(x => x.Address = new Uri($"http://1.1.1.1:8500"));

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter =  TimeSpan.FromSeconds(5),// 服务启动后多久注册
                Interval = TimeSpan.FromSeconds(10),// 健康检查时间间隔，或者称为心跳间隔
                //HTTP = $"http://{Program.IP}:{Program.Port}/health",//健康检查地址
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new []{ httpCheck},
                ID = Guid.NewGuid().ToString(),
                Name = "YourServiceName",
                Address = "Your consul IP",
                Port = 8500,
                Tags = new []{$""}
            };

            //服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
            consulClient.Agent.ServiceRegister(registration).Wait();
            //服务停止时取消注册
            lifetime.ApplicationStopping.Register(() => { consulClient.Agent.ServiceDeregister(registration.ID).Wait(); })

            return app;
        }
    }
}
