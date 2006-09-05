"""
Myro Base Classes.
(c) 2006, Institute for Personal Robots in Education
http://roboteducation.org/
Distributed under a Shared Source License
"""

__REVISION__ = "$Revision$"
__BUILD__    = "$Build: 5 $"
__VERSION__  = "0.1." + __BUILD__.split()[1]
__AUTHOR__   = "Doug Blank <dblank@brynmawr.edu>"

import sys, atexit, posixpath, time
import myro.globals

class Robot(object):
    def __init__(self):
        """
        Base robot class.
        """
        self.devices = []

    def translate(self, amount):
        raise AttributeError, "this method needs to be written"

    def rotate(self, amount):
        raise AttributeError, "this method needs to be written"

    def move(self, translate, rotate):
        raise AttributeError, "this method needs to be written"

    def forward(self, amount):
        raise AttributeError, "this method needs to be written"

    def backward(self, amount):
        raise AttributeError, "this method needs to be written"

    def left(self, amount):
        raise AttributeError, "this method needs to be written"

    def right(self, amount):
        raise AttributeError, "this method needs to be written"

    def quit(self):
        raise AttributeError, "this method needs to be written"

    def stop(self):
        raise AttributeError, "this method needs to be written"

class Scribbler(Robot):
    def __init__(self, id):
        Robot.__init__(self)
        self.id = id
    # add Bluetooth/Serial interface stuff here

class SimScribbler(Scribbler):
    def __init__(self, id):
        import myro.simulator
        Scribbler.__init__(self, id)
        globalspath, filename = posixpath.split(myro.globals.__file__)
        myro.globals._myropath, directory = posixpath.split(globalspath)
        self.sim = myro.simulator.INIT(
            posixpath.join(myro.globals._myropath, "worlds", "MyroWorld"))
        for port in self.sim.ports:
            print "Simulator starting listener on port", port, "..."
            thread = myro.simulator.Thread(self.sim, port)
            thread.start()
        # start the client(s):
        from myro.robot.symbolic import TCPRobot
        self.client = []
        for port in self.sim.ports:
            self.client.append(TCPRobot("localhost", port))
        myro.globals._robot = self
        myro.globals._simulator = self.sim
        atexit.register(_cleanup) # FIX: hack to get _cleanup called before Tk exitfunc, which hangs
    def translate(self, amount):
        return self.client[0].translate(amount)
    def rotate(self, amount):
        return self.client[0].rotate(amount)
    def move(self, translate, rotate):
        return self.client[0].move(translate, rotate)
    def forward(self, amount):
        return self.client[0].translate(amount)
    def backward(self, amount):
        return self.client[0].translate(-amount)
    def left(self, amount):
        return self.client[0].rotate(amount)
    def right(self, amount):
        return self.client[0].rotate(-amount)
    def stop(self):
        return self.client[0].move(0, 0)
    def quit(self):
        return self.client[0].move("quit")

# functions:
def _cleanup():
    if myro.globals._robot != None:
        myro.globals._robot.quit()
    if myro.globals._simulator != None:
       myro.globals._simulator.destroy()

# Get ready for user prompt; set up environment:
if not myro.globals._setup:
    myro.globals._setup = 1
    atexit.register(_cleanup)
    # Ok, now we're ready!
    print "Myro, (c) 2006 Institute for Personal Robots in Education"
    print "Version %s, ready!" % (__VERSION__)
