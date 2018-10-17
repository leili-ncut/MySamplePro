using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MySamplePro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static int _count = 0;
        private IConfiguration _configuration;
        private IConfig anotherConfig;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet,Route("apollo")]
        public string GetApolloTest()
        {
            //var timeout = _configuration.GetValue<string>("timeout");
            //var timeout = _configuration.GetValue("timeout", "");
            var manger = new ApolloConfigurationManager();
            var config = manger.GetAppConfig().Result;
            anotherConfig = manger.GetConfig("ClientService").Result;
            // namespaceName
            // manager.GetConfig("namespaceName").Result;
            // 配置更改事件
            config.ConfigChanged += Config_ConfigChanged;
            var timeout = config.GetProperty("Service.Port", "");
            if (string.IsNullOrEmpty(timeout))
            {
                timeout = anotherConfig.GetProperty("Service.Port", "");
            }
            return timeout;
        }
        private void Config_ConfigChanged(object sender, ConfigChangeEventArgs args)
        {
            // 更改的配置
            foreach (var c in args.Changes)
            {
                var change = c.Value;
                // change.ChangeType
                // change.PropertyName
                // change.NewValue
                // change.OldValue
            }
        }

        [HttpGet,Route("sex")]
        public string GetSex(string name)
        {
            if (name == "lil")
                return "Man";
            return "Woman";
        }

        [HttpGet,Route("age")]
        public int? GetAge(string name)
        {
            if (name == "lil")
                return 26;
            return null;
        }

        [Authorize]
        [HttpGet, Route("auth")]
        public string GetAuth()
        {
            return "auth test";
        }

        [HttpGet,Route("discovery")]
        public string GetDiscovery()
        {
            _count++;
            System.Console.WriteLine($"get...{_count}");
            if (_count <= 3)
            {
                Thread.Sleep(5000);
            }
            return "discovery";
        }
    }
}