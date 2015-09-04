from azure.servicebus import ServiceBusService

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

#initializa led
led = mraa.Gpio(14)
led.dir(mraa.DIR_OUT)
led.write(1)

#even hub config
sbs = ServiceBusService("myhubed-ns",
                        shared_access_key_name="SendReceiveRule",
                        shared_access_key_value="hGRlK8cfxw/nU0SoH/egwJbg33BiUAi
# Listen for incoming connections
sock.listen(5)
print "Connection opened..."

logging = False
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
            if msg == "blink" :
                led.write(0)
                time.sleep(0.1)
                led.write(1)
                    print "blinked"
            elif msg == "startlogging":
                print "log started"
                logging = True
            elif msg == "stoplogging":
                print "log stopped"
                logging = False
            elif msg == "shutdown":
                conn.close()
                sys.exit()
            else : print "No such command"

        j=0
        if logging :
            from wifi import Cell, Scheme
            cells = Cell.all('wlan0')
            count = len(cells)
            print "Iteration: " + str(j)
            print "Count: " + str(count)
            sbs.send_event('myhub',  json.dumps("Iteration: " + str(j)))
            sbs.send_event('myhub',  json.dumps("Count: " + str(count)))
            j=j+1
            for i in range(count):
                message = "SSID: " + cells[i].ssid + ", Signal: " + str(cells[i]
                sbs.send_event('myhub',  json.dumps(message))
                led.write(0)
                print message
                time.sleep(2)
                led.write(1)

    finally:
        conn.close()