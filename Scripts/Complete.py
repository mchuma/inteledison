//blink
import socket
import sys
import mraa
import time

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the port
server_address = ('0.0.0.0', 5005)
print >>sys.stderr, 'starting up on %s port %s' % server_address
sock.bind(server_address)

led = mraa.Gpio(14)
led.dir(mraa.DIR_OUT)
led.write(1)

# Listen for incoming connections
sock.listen(5)

while True:
    try:
        # Wait for a connection
        conn, addr = sock.accept()
        data = conn.recv(20)
        if not data: break
        led.write(0)
        time.sleep(0.1)
        led.write(1)
    finally:
        conn.close()
//eventhub
from azure.servicebus import ServiceBusService

import mraa
import time
import time
import json
import random

sbs = ServiceBusService("myhubed-ns",
                        shared_access_key_name="SendReceiveRule",
                        shared_access_key_value="hGRlK8cfxw/nU0SoH/egwJbg33BiUAi

from wifi import Cell, Scheme
j=0
led = mraa.Gpio(14)
led.dir(mraa.DIR_OUT)
led.write(1)
while True:
    cells = Cell.all('wlan0')
    count = len(cells)
    print "Count: " + str(count)
    sbs.send_event('myhub',  json.dumps("Iteration: " + str(j)))
    sbs.send_event('myhub',  json.dumps("Count: " + str(count)))
    j=j+1
    for i in range(count):
        message = "SSID: " + cells[i].ssid + ", Signal: " + str(cells[i].signal)
        sbs.send_event('myhub',  json.dumps(message))
        led.write(0)
        print message
        time.sleep(2)
        led.write(1)