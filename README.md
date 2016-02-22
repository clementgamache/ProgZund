# ProgZund
A desktop application used to control old plotters using RS232 protocols. Tested on a Zund M-800 machine and a Windows 10 computer.

This machine was first used to cut cardboard mats (passe partout) and the used software could only do that.
The company that owns this machine started to need to create stencils to increase their production.
Because the machine uses RS232 protocols, no existing software was compatible with this machine, so I was asked to create one. 

A first version was written in Python, using PyQt for the user interface and object-oriented programming for the logic.
The application worked perfectly, but only to do complex shapes from plotter (.plt) files.

Since I don't like PyQt, I decided to rewrite the application in C# in which I would add the following features:

Better UI
Dynamic refreshing of the preview of the current work
Mat (passe partout) cutting
Complex mat cutting (aka a rectangle cut containing many rectangle cuts)
