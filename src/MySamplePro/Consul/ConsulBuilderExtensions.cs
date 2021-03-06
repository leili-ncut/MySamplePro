﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace MySamplePro.Consul
{
    public static class ConsulBuilderExtensions
    {
        //步骤 1.服务注册
        //2. 在Starup类的Configure方法中，调用此扩展方法
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, HealthService healthService, ConsulService consulService)
        {

            var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{consulService.IP}:{consulService.Port}"));//请求注册的 Consul 地址

            var httpCheck = new AgentServiceCheck()

            {

                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册

                Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔

                HTTP = $"http://{healthService.IP}:{healthService.Port}/api/health",//健康检查地址

                Timeout = TimeSpan.FromSeconds(5)

            };

            // Register service with consul

            var registration = new AgentServiceRegistration()

            {

                Checks = new[] { httpCheck },

                ID = healthService.Name + "_" + healthService.Port,

                Name = healthService.Name,

                Address = healthService.IP,

                Port = healthService.Port,

                Tags = new[] { $"urlprefix-/{healthService.Name}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别

            };

            consulClient.Agent.ServiceRegister(registration).Wait();//服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）

            lifetime.ApplicationStopping.Register(() =>

            {

                consulClient.Agent.ServiceDeregister(registration.ID).Wait();//服务停止时取消注册

            });

            return app;

        }

    }

    public class ConsulService
    {
        public string IP { get; set; }

        public int Port { get; set; }

    }

    public class HealthService
    {
        public string Name { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }
    }
}
