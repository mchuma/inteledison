using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;

namespace EdisonMonitor
{
    public class SimpleEventProcessor : IEventProcessor
    {
        Stopwatch checkpointStopWatch;
        public static List<string> Messages;

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            //Console.WriteLine(string.Format("Processor Shuting Down. Partition ‘{0}’, Reason: ‘{1}’.", context.Lease.PartitionId, reason.ToString()));
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            //Console.WriteLine(string.Format("SimpleEventProcessor initialize. Partition: ‘{0}’, Offset: ‘{1}'", context.Lease.PartitionId, context.Lease.Offset));
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
                //Console.WriteLine(string.Format("Message received. Partition: ‘{0}’, Data: ‘{1}'", context.Lease.PartitionId, data));
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(2))
            {
                await context.CheckpointAsync();
                lock (this)
                {
                    this.checkpointStopWatch.Reset();
                }
            }
        }

    }
}