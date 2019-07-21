# serilog-sinks-signalr

A Serilog sink for ASP.NET CORE that writes events to a SignalR Hub.

This library is a port of the original [serilog-sinks-signalr](https://github.com/serilog/serilog-sinks-signalr) that support [Microsoft.AspNetCore.SignalR](https://www.nuget.org/packages/Microsoft.AspNetCore.SignalR/)

## Configuration from hub application

From within the SignalR server application with a hub retrieved from the application services:

```csharp
var logHub = app.ApplicationServices.GetService<IHubContext<Serilog.Sinks.SignalR.LogHub, ILogEventClient>>();

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Verbose()
.WriteTo.SignalR(logHub,
  Serilog.Events.LogEventLevel.Information,
  groupNames: new[] { "CustomGroup"}, // default is null
  userIds: new[] { "JaneD1234" }, // default is null
  excludedConnectionIds: new[] { "12345", "678910" }) // default is null
.CreateLogger();
```

## Configuration from other clients

From any client application with a hub hosted at `http://localhost:8080` and a hub implemented named `MyHub`:

```csharp
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Verbose()
.WriteTo.SignalRClient("http://localhost:8080",
  Serilog.Events.LogEventLevel.Information,
  hub: "MyHub" // default is LogHub
  groupNames: new[] { "CustomGroup"}, // default is null
  userIds: new[] { "JaneD1234" }) // default is null
.CreateLogger();
```

### SignalR Server
Create a hub class with any name that ends in `Hub` or use the default name `LogHub`. Then create a method named `receiveLogEvent`, which is capable of accepting all data from the sink.
```csharp
public class MyHub : Hub
{
  public void receiveLogEvent(string[] groups, string[] userIds, Serilog.Sinks.SignalR.Data.LogEvent logEvent)
  {
    // send to all clients
    Clients.All.sendLogEvent(logEvent);
    // just the specified groups
	Clients.Groups(groups).sendLogEvent(logEvent);
    // just the specified users
    Clients.Users(users).sendLogEvent(logEvent);
  }
}
```

## Receiving the log event
Set up a SignalR client and subscribe to the `sendLogEvent` method.

```csharp
var connection = new HubConnection("http://localhost:8080");
var hubProxy = connection.CreateHubProxy("MyHub");

hubProxy.On<Serilog.Sinks.SignalR.Data.LogEvent>("sendLogEvent", (logEvent) =>
{
  Console.WriteLine(logEvent.RenderedMessage);
});
```