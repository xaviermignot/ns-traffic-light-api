using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Business.Logic;
using TrafficLight.Api.Configuration;
using TrafficLight.Api.Hubs;
using TrafficLight.Api.Tasks.Contract;
using TrafficLight.Api.Tasks.Logic;
using Tweetinvi;
using Tweetinvi.Models;

namespace TrafficLight.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddJsonOptions(opt =>
            {
                opt.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });
            services.AddSignalR(opt =>
            {
                opt.Hubs.EnableDetailedErrors = true;
            });

            services.Configure<AzureSettings>(Configuration.GetSection("Azure"));
            services.Configure<TwitterSettings>(Configuration.GetSection("TwitterApi:Keywords"));

            services.AddTransient<ITrafficLightService, TrafficLightService>();
            services.AddTransient<IMessagingService, StorageQueueMessagingService>();

            services.AddSingleton<ITweetBackgroundWatcher, TweetBackgroundWatcher>();

            Auth.ApplicationCredentials = new TwitterCredentials(
                Configuration["TwitterApi:Credentials:ConsumerKey"],
                Configuration["TwitterApi:Credentials:ConsumerSecret"],
                Configuration["TwitterApi:Credentials:Token"],
                Configuration["TwitterApi:Credentials:TokenSecret"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifeTime,
            IServiceProvider serviceProvider,
            ITweetBackgroundWatcher tweetWatcher)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            appLifeTime.ApplicationStarted.Register(() => tweetWatcher.StartWatching());
            appLifeTime.ApplicationStopping.Register(() => tweetWatcher.StopWatching());

            app.UseMvc();

            app.UseSignalR();
        }
    }
}
