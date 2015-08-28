#include <WiFi.h>
#include <SPI.h>
#include <Ethernet.h>

char ssid[] = "EleksPublic";     // the name of your network
int status = WL_IDLE_STATUS;     // the Wifi radio's status
const char *server="inteledisoneventhub-ns.servicebus.windows.net";
const char *sas = "SharedAccessSignature sr=https%3a%2f%2finteledisoneventhub-ns.servicebus.windows.net%2fedisoneventhub%2fpublishers%2f1%2fmessages&sig=1yqSTacHLmupUxkoo4fECw0uSdLuet4l%2bjbBcAkEhco%3d&se=1440759240&skn=SendReceiveRule";
const char *serviceNamespace = "inteledisoneventhub-ns";
const char *hubName = "edisoneventhub";
const char *deviceName = "1";

EthernetClient client;
char buffer[256];

void setup() {
  //Initialize serial and wait for port to open:
  Serial.begin(9600); 
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }
  
  // check for the presence of the shield:
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present"); 
    // don't continue:
    while(true);
  } 

  String fv = WiFi.firmwareVersion();
  if( fv != "1.1.0" )
    Serial.println("Please upgrade the firmware");
  
   
    
 // attempt to connect to Wifi network:
  while ( status != WL_CONNECTED) {  
     // scan for existing networks:
     Serial.println("Scanning available networks...");
     listNetworks();
   
    Serial.print("Attempting to connect to open SSID: ");
    Serial.println(ssid);
    status = WiFi.begin(ssid);

    // wait 10 seconds for connection:
    delay(10000);
    
  }
   
  // you're connected now, so print out the data:
  Serial.print("You're connected to the network");
  printCurrentNet();
  printWifiData();
}

void loop() {
  // check the network connection once every 10 seconds:
  delay(10000);
 // printCurrentNet();
  send_requestAzureEventHub(777);
  wait_responseAzureEventHub();
  read_responseAzureEventHub();
  end_requestAzureEventHub();
}

void printWifiData() {
  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
    Serial.print("IP Address: ");
  Serial.println(ip);
  Serial.println(ip);
  
  // print your MAC address:
  byte mac[6];  
  WiFi.macAddress(mac);
  Serial.print("MAC address: ");
  Serial.print(mac[5],HEX);
  Serial.print(":");
  Serial.print(mac[4],HEX);
  Serial.print(":");
  Serial.print(mac[3],HEX);
  Serial.print(":");
  Serial.print(mac[2],HEX);
  Serial.print(":");
  Serial.print(mac[1],HEX);
  Serial.print(":");
  Serial.println(mac[0],HEX);
  
  // print your subnet mask:
  IPAddress subnet = WiFi.subnetMask();
  Serial.print("NetMask: ");
  Serial.println(subnet);

  // print your gateway address:
  IPAddress gateway = WiFi.gatewayIP();
  Serial.print("Gateway: ");
  Serial.println(gateway);
}

void printCurrentNet() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print the MAC address of the router you're attached to:
  byte bssid[6];
  WiFi.BSSID(bssid);    
  Serial.print("BSSID: ");
  Serial.print(bssid[5],HEX);
  Serial.print(":");
  Serial.print(bssid[4],HEX);
  Serial.print(":");
  Serial.print(bssid[3],HEX);
  Serial.print(":");
  Serial.print(bssid[2],HEX);
  Serial.print(":");
  Serial.print(bssid[1],HEX);
  Serial.print(":");
  Serial.println(bssid[0],HEX);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.println(rssi);

  // print the encryption type:
  byte encryption = WiFi.encryptionType();
  Serial.print("Encryption Type:");
  Serial.println(encryption,HEX);
}

void listNetworks() {
  // scan for nearby networks:
  Serial.println(" Scan Networks ");
  int numSsid = 0;
  while (numSsid == 0)
  {
    numSsid = WiFi.scanNetworks();
    if (numSsid == -1)
    { 
      Serial.println("Couldn't get a wifi connection");
      while(true);
    } 
  
    // print the list of networks seen:
    Serial.print("number of available networks:");
    Serial.println(numSsid);
    if(numSsid == 0)
      Serial.println("waiting for WiFi networks detection...");
    delay(3000);
  }
  // print the network number and name for each network found:
  for (int thisNet = 0; thisNet<numSsid; thisNet++) {
    Serial.print(thisNet);
    Serial.print(") ");
    Serial.print(WiFi.SSID(thisNet));
    Serial.print("\tSignal: ");
    Serial.print(WiFi.RSSI(thisNet));
    Serial.print(" dBm");
    Serial.print("\tEncryption: ");
    printEncryptionType(WiFi.encryptionType(thisNet));
  }
}

  void printEncryptionType(int thisType) {
  // read the encryption type and print out the name:
  switch (thisType) {
  case ENC_TYPE_WEP:
    Serial.println("WEP");
    break;
  case ENC_TYPE_TKIP:
    Serial.println("WPA");
    break;
  case ENC_TYPE_CCMP:
    Serial.println("WPA2");
    break;
  case ENC_TYPE_NONE:
    Serial.println("None");
    break;
  case ENC_TYPE_AUTO:
    Serial.println("Auto");
    break;
  }
}

void send_requestAzureEventHub(int value)
{
Serial.println("\nconnecting…");

if (client.connect(server, 80)) {

Serial.print("sending ");
Serial.println(value);

// POST URI

sprintf(buffer, "POST /edisoneventhub/publishers/%s/messages HTTP/1.1", deviceName);
client.println(buffer);

Serial.println(buffer);

// Host header
sprintf(buffer, "Host: %s", server);
client.println(buffer);
Serial.println(buffer);

// Application key
sprintf(buffer, "Authorization: %s", sas);
client.println(buffer);
Serial.println(buffer);

// content type
client.println("Content-Type: application/atom+xml;type=entry;charset=utf-8");
Serial.println("Content-Type: application/atom+xml;type=entry;charset=utf-8");

// POST body
sprintf(buffer, "{\"value\": %s}", "Hello World from Edison");
// Content length
client.print("Content-Length: ");
Serial.println("Content-Length: ");
client.println(strlen(buffer));
Serial.println(strlen(buffer));


// End of headers
client.println();
Serial.println();
// Request body
client.println(buffer);
Serial.println(buffer);

Serial.println("sending finished");
} else {
Serial.println("connection failed");
}
}

/*
** Wait for response
*/

void wait_responseAzureEventHub()
{
while (!client.available()) {
if (!client.connected()) {
return;
}
}
}

/*
** Read the response and dump to serial
*/

void read_responseAzureEventHub()
{
bool print = true;
Serial.println("printing responce... ");
while (client.available()) {
char c = client.read();
// Print only until the first carriage return
if (c == '\n'){
print = false;
}

if (print)
Serial.print(c);
}
if(!client.available()){
char c = client.read();
Serial.print(c);
}
}

/*
** Close the connection
*/

void end_requestAzureEventHub()
{
client.stop();
}
