# Traffic Light Api

This is a simple Api written in aspnet core to simulate the behavior of a traffic light.
The Api is used by the [Traffic Light Bot](https://github.com/xaviermignot/ns-traffic-light-bot) and polled by a UWP application running on a Raspberry Pi to control a "real" traffic light.

The global purpose of this project is just to have fun and present cool tech at local events and meetups.

## How to run the Api

* Install [.NET Core](https://www.microsoft.com/net/core) if you haven't already 
* Clone the repo
* Restore the packages with a `dotnet restore` command
* Run the app with a `dotnet watch run` command, this will run a background process watching for your changes in the code and automagically compile them

## How to use the Api

* Perform a **GET** on the route `api/trafficlight` to get the state of the traffic light. The state can be `Off`, `Green`, `Orange` or `Red` 
* Perform a **PUT** on the route `api/trafficlight/{color}` to light a bulb, the value of `{color}` can be `green`, `orange` or `red`
* Perform a **DELETE** on the route `api/trafficlight` to switch the light off

## Twitter integration

At the app startup, an instance of the `TweetBackgroundWatcher` class is created and starts watching for tweets with specific hashtags in order to change the traffic light state.
The twitter Api credentials and the hashtags can be configured in the `appsettings.json` file.