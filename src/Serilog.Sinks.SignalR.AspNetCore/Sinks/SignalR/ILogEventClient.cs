using System.Threading.Tasks;
using Serilog.Sinks.SignalR.Data;

namespace Serilog.Sinks.SignalR
{
    public interface ILogEventClient
    {
        Task ReceiveLogEvent(LogEvent logEvent);
    }
}