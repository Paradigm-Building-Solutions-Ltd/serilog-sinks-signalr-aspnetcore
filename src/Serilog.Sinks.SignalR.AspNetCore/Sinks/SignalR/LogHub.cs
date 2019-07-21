using Microsoft.AspNetCore.SignalR;

namespace Serilog.Sinks.SignalR
{
    public class LogHub : Hub<ILogEventClient>
    {

    }
}