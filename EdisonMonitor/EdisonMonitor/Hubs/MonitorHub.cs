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
        public static List<string> Messages { get; set; }

        public void SendMessage(string message)
        {
            //RefreshData();
            // Call the broadcastMessage method to update clients.
            Clients.All.showMessageOnClient(message  + DateTime.Now.ToString());  //GetMessageText());
        }

        public void RefreshData()
        {
            Messages = new List<string>();
            string eventHubConnectionString = "Endpoint=sb://inteledisoneventhub-ns.servicebus.windows.net/;SharedAccessKeyName=SendReceiveRule;SharedAccessKey=29SkhvIFngQqIJ8Cv9Lz0Luo+EFVsyZYIycUNaB3ffM=";
            string eventHubName = "edisoneventhub";
            string storageAccountName = "edisoneventhubstorage";
            string storageAccountKey = "wH/HCkHLoPH93xCZwFMR4V4OZE7nbmps1+sFaqutt1mhBjdVdWd+ECuo+c0GbmhB9RsOxFiCB4XEsxoizMksAw==";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
            storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            var a = eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();
            if (!a.IsCompleted)
            {
                Clients.All.showMessageOnClient("Waiting..." + DateTime.Now.ToString());
                Thread.Sleep(1000);

            }
            Clients.All.showMessageOnClient(GetMessageText() + DateTime.Now.ToString());
        }

        private string GetMessageText()
        {
            string result = "abc";
            if (SimpleEventProcessor.Messages != null && SimpleEventProcessor.Messages.Count > 0)
            {
                foreach (var messageItem in SimpleEventProcessor.Messages)
                {
                    result += messageItem + Environment.NewLine;
                }
            }

            return result;
        }
    }
}