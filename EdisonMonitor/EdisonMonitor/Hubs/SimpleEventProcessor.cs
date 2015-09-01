using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Hubs;

namespace EdisonMonitor
{
    public class SimpleEventProcessor : IEventProcessor
    {
        Stopwatch checkpointStopWatch;
        private static List<string> Messages;
        public static IHubCallerConnectionContext<dynamic> Clients;

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            if (Messages == null || Messages.Count() == 0)
            {
                Messages = new List<string>();
            }

            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Messages.Add(data);

            }

            if (Messages.Count > 0 && Clients != null)
            {
                Clients.All.showMessageOnClient(GetMessageText());
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                lock (this)
                {
                    this.checkpointStopWatch.Reset();
                }
            }
        }

        private string GetMessageText()
        {
            string result = "";
            if (Messages != null && Messages.Count > 0)
            {
                foreach (var messageItem in Messages)
                {
                    result += messageItem + Environment.NewLine;
                }
            }

            return result;
        }

    }
}