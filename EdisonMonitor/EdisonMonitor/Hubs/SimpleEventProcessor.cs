﻿using System;
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
        private static List<string> MessagesReceived;
        public static IHubCallerConnectionContext<dynamic> Clients;
        public static EventProcessorHost Host;

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                Clients.All.showMessageOnClient("XClose");
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
            MessagesReceived = new List<string>();
            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                MessagesReceived.Add(data);
            }

            if (MessagesReceived.Count > 0 && Clients != null)
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
            if (MessagesReceived != null && MessagesReceived.Count > 0)
            {
                foreach (var messageItem in MessagesReceived)
                {
                    result += messageItem + Environment.NewLine;
                }
            }

            return result;
        }

    }
}