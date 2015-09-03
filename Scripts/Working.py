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