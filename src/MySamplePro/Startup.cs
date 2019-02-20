using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Consul;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySamplePro.Model;
using MySamplePro.Services;
using Steeltoe.Extensions.Configuration.ConfigServer;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MySamplePro.Consul;
using Polly;
using Swashbuckle.AspNetCore.Swagger;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Info = MySamplePro.Model.Info;

namespace MySamplePro
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            //// 1. 使用 spring cloud configuration 
            //services.ConfigureConfigServerClientOptions(Configuration);
            //// 2. 使用 spring cloud configuration 
            //services.AddConfiguration(Configuration);


            //// 3. 使用 spring cloud configuration 
            //services.Configure<ConfigServerData>(Configuration);

            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("fallback"),
                StatusCode = System.Net.HttpStatusCode.TooManyRequests
            };
            //4. 使用 HttpClientFactory,polly实现重试，断路器模式
            services.AddHttpClient<IValueService, ValueService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy())
                //.AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().FallbackAsync(fallbackResponse, // fall back 
                //    async b =>
                //    {
                //        // Logger.LogWarning($"fallback here {b.Exception.Message}");
                //    }))
                //.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1)) // timeout 
                ;
            //5. 使用consul 注册服务
            services.AddSingleton<IHostedService, ConsulHostedService>();
            services.Configure<ConsulConfig>(Configuration.GetSection("ConsulConfig"));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = Configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));
            services.AddSingleton<Func<IConsulClient>>(p => () => new ConsulClient(consulConfig =>
            {
                var address = Configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));
            // 6. Auth 
            var audienceConfig = Configuration.GetSection("Audience");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = "TestKey";
                })
                .AddJwtBearer("TestKey", x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.TokenValidationParameters = tokenValidationParameters;
                });

            var version = Configuration["Swagger.Version"];

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory,IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();

            // auth
            app.UseAuthentication();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            #region register this service

            //ConsulService consulService = new ConsulService()

            //{

            //    IP = Configuration["Consul:IP"],

            //    Port = Convert.ToInt32(Configuration["Consul:Port"])

            //};

            //HealthService healthService = new HealthService()

            //{

            //    IP = Configuration["Service:IP"],

            //    Port = Convert.ToInt32(Configuration["Service:Port"]),

            //    Name = Configuration["Service:Name"],

            //};

            //app.RegisterConsul(lifetime, healthService, consulService);

            #endregion



        }
    }
}
