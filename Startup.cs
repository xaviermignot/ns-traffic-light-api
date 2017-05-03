using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrafficLight.Api.Hubs;
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

            Auth.ApplicationCredentials = new TwitterCredentials(
                Configuration["TwitterApi:Credentials:ConsumerKey"],
                Configuration["TwitterApi:Credentials:ConsumerSecret"],
                Configuration["TwitterApi:Credentials:Token"],
                Configuration["TwitterApi:Credentials:TokenSecret"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifeTime, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            TweetBackgroundWatcher.Initialize(serviceProvider.GetService<IConnectionManager>());

            TweetBackgroundWatcher.LazyInstance.Value.RedLightTrack = Configuration["TwitterApi:Keywords:RedLight"];
            TweetBackgroundWatcher.LazyInstance.Value.OrangeLightTrack = Configuration["TwitterApi:Keywords:OrangeLight"];
            TweetBackgroundWatcher.LazyInstance.Value.GreenLightTrack = Configuration["TwitterApi:Keywords:GreenLight"];

            appLifeTime.ApplicationStarted.Register(() => TweetBackgroundWatcher.LazyInstance.Value.StartWatching());
            appLifeTime.ApplicationStopping.Register(() => TweetBackgroundWatcher.LazyInstance.Value.StopWatching());

            app.UseMvc();

            app.UseSignalR();
        }
    }
}
