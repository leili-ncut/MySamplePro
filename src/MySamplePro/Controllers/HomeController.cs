using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySamplePro.Model;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace MySamplePro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IOptionsSnapshot<ConfigServerData> IConfigServerData { get; set; }

        private ConfigServerClientSettingsOptions ConfigServerClientSettingsOptions { get; set; }

        private IConfigurationRoot Config { get; set; }

        public HomeController(IConfigurationRoot config,IOptionsSnapshot<ConfigServerData> configServerData, IOptions<ConfigServerClientSettingsOptions> confgServerSettings)
        {
            if (configServerData != null)
                IConfigServerData = configServerData;
            if (confgServerSettings != null)
            {
                ConfigServerClientSettingsOptions = confgServerSettings.Value;
            }
        }

        public IActionResult Index()
        {
            return  View();
        }

        public IActionResult ConfigServer()
        {
            CreateConfigServerDataViewData();
            return View();
        }

        private void CreateConfigServerDataViewData()
        {
            ViewData["ASPNETCORE_ENVIRONMENT"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // IConfigServerData property is set to a IOptionsSnapshot<ConfigServerData> that has been
            // initialized with the configuration data returned from the Spring Cloud Config Server
            if (IConfigServerData != null && IConfigServerData.Value != null)
            {
                var data = IConfigServerData.Value;
                ViewData["Bar"] = data.Bar ?? "Not returned";
                ViewData["Foo"] = data.Foo ?? "Not returned";

                ViewData["Info.Url"] = "Not returned";
                ViewData["Info.Description"] = "Not returned";

                if (data.Info != null)
                {
                    ViewData["Info.Url"] = data.Info.Url ?? "Not returned";
                    ViewData["Info.Description"] = data.Info.Description ?? "Not returned";
                }
            }
            else
            {
                ViewData["Bar"] = "Not Available";
                ViewData["Foo"] = "Not Available";
                ViewData["Info.Url"] = "Not Available";
                ViewData["Info.Description"] = "Not Available";
            }

        }
    }
}