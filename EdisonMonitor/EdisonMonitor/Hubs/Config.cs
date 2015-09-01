using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdisonMonitor
{
    public class Config
    {
        public const string EVENT_HUB_CONNECTION_STRING = "Endpoint=sb://myhubed-ns.servicebus.windows.net/;SharedAccessKeyName=SendReceiveRule;SharedAccessKey=hGRlK8cfxw/nU0SoH/egwJbg33BiUAi6b40P0uu9qNU=";
        public const string EVENT_HUB_NAME = "myhub";
        public const string STORAGE_ACCOUNT_NAME = "edisoneventhubstorage";
        public const string STORAGE_ACCOUNT_KEY = "wH/HCkHLoPH93xCZwFMR4V4OZE7nbmps1+sFaqutt1mhBjdVdWd+ECuo+c0GbmhB9RsOxFiCB4XEsxoizMksAw==";

        public static string GetStorageConnectionString()
        {
            return string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", STORAGE_ACCOUNT_NAME, STORAGE_ACCOUNT_KEY);
        }
    }
}