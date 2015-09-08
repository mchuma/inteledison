from azure.servicebus import ServiceBusService
from wifi import Cell, Scheme
import socket
import sys
import mraa
import time
import time
import json
import random

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the port
server_address = ('0.0.0.0', 5005)
sock.bind(server_address)

#initialize led
led = mraa.Gpio(14)
led.dir(mraa.DIR_OUT)

def signal(secs):
    led.write(0)
    time.sleep(secs)
    led.write(1)

signal(1)

#even hub config
sbs = ServiceBusService("myhubed-ns",
                        shared_access_key_name="SendReceiveRule",
                        shared_access_key_value="hGRlK8cfxw/nU0SoH/egwJbg33BiUAi

def logging():
    cells = Cell.all('wlan0')
    count = len(cells)
    sbs.send_event('myhub',  json.dumps("Count: " + str(count)))
    print "Count: " + str(count)
    for i in range(count):
        message = "SSID: " + cells[i].ssid + ", Signal: " + str(cells[i].signal)
        sbs.send_event('myhub',  json.dumps(message))
        print message
    signal(1)
    print "Done"
	
# Listen for incoming connections
sock.listen(5)
print "Connection opened..."

while True:
    try:
        # Wait for a connection
        conn, addr = sock.accept()
        data = conn.recv(1024)
        if not data:
            print "No Data..."
            break
        else:
            msg = data.decode().strip()
            print "Command: " + msg
            if msg == "led-on" :
                led.write(0)
            elif msg == "led-off":
                led.write(1)
            elif msg == "logging":
                logging()
            elif msg == "shutdown":
                conn.close()
                sys.exit()
            else : print "No such command"

    finally:
        conn.close()

