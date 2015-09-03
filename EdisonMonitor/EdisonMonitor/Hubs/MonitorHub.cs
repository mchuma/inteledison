using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;
using System.Threading;

namespace EdisonMonitor
{
	public class MonitorHub : Hub
	{
        EventProcessorHost eventProcessorHost;

        public void Start()
        {
            string eventProcessorHostName = Guid.NewGuid().ToString();
            eventProcessorHost = new EventProcessorHost(eventProcessorHostName, Config.EVENT_HUB_NAME, 
                EventHubConsumerGroup.DefaultGroupName, Config.EVENT_HUB_CONNECTION_STRING, Config.GetStorageConnectionString());
            SimpleEventProcessor.Clients = this.Clients;
            SimpleEventProcessor.Host = eventProcessorHost;

            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();
        }

        public void Stop()
        {
            if (eventProcessorHost != null)
            {
                eventProcessorHost.UnregisterEventProcessorAsync();
            }
            else if (SimpleEventProcessor.Host != null)
            {
                SimpleEventProcessor.Host.UnregisterEventProcessorAsync();
            }
        }
    }
}