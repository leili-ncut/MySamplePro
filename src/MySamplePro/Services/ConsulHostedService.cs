using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySamplePro.Model;

namespace MySamplePro.Services
{
    public class ConsulHostedService : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;
        private readonly IConsulClient _consulClient;
        private readonly IOptions<ConsulConfig> _consulConfig;
        private readonly ILogger<ConsulHostedService> _logger;
        private readonly IServer _server;
        private string _registrationID;

        public ConsulHostedService(
            IConsulClient consulClient,
            IOptions<ConsulConfig> consulConfig,
            ILogger<ConsulHostedService> logger,
            IServer server)
        {
            _server = server;
            _logger = logger;
            _consulConfig = consulConfig;
            _consulClient = consulClient;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var features = _server.Features;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();
            //var uri = new Uri(address);
            var ip = GetIPAddress();
            var uri = new Uri($"http://{ip}");
            _registrationID = $"{_consulConfig.Value.ServiceID}-{64038}";

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationID,
                Name = _consulConfig.Value.ServiceName,
                Address = ip,
                //Address = "192.168.111.182",  // **特别标注：此处地址不能带scheme，只用host，否则ocelot不能服务发现**
                Port = 64038,
                Tags = new[] { "Consul", "mysample-Demo" },
                Check = new AgentServiceCheck()
                {
                    HTTP = $"{uri.Scheme}://{uri.Host}:64038/api/health/status",
                    //HTTP = "http://www.baidu.com",//for test
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            //_logger.LogInformation("Registering in Consul");
            await _consulClient.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        private string GetIPAddress()
        {
            var ip = string.Empty;
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipa.ToString();
                    break;
                }
            }

            return ip;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            //_logger.LogInformation("Deregistering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationID, cancellationToken);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Deregisteration failed");
            }
        }
    }
}
