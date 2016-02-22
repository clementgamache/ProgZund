# -*- coding: cp1252 -*-
import sys
from PyQt4 import QtCore, QtGui, uic

from files import *
import os
import turtle




class Trace:
  
    def __init__(self, decalageX, decalageY, largeurPouces, numeroPort, fileName):
        self.decalageX = decalageX
        self.decalageY = decalageY
        self.largeurImage = largeurPouces
        self.numeroPort = numeroPort
        self.fileName = fileName
        self.POINTSPARPOUCE = 2540.0
        self.fichierDebut = "C:r\Python34\Lib\Plotter\BEGIN.txt"
        self.fichierFin = "C:\Python34\Lib\Plotter\END.txt"
        self.fichierSortie = "resultat.plt"

    def genererTuples(self):
        self.lines = []
        with open(self.fileName, 'r') as f:
            data = f.readlines()
            for line in data:
                line.replace(',', ' ')
                l = []
                line = line.strip("\n").strip(';')
                l.append(line[:2])
                rest = line[2:]
                division = rest.split(' ')
                if len(division) > 1:
                    l.append(int(division[0]))
                    l.append(int(division[1]))
                self.lines.append(l)
                
        
        i=0
        while i < len(self.lines):
            if len(self.lines[i]) != 3:
                self.lines.remove(self.lines[i])
            else:
                i+=1
    def definirMinMax(self):
        self.minX = 9999999
        self.maxX = -9999999
        self.minY = 999999999
        self.maxY = -9999999

        for line in self.lines:
            if line[1] < self.minX:
                self.minX = line[1]
            if line[1] > self.maxX:
                self.maxX = line[1]
            if line[2] < self.minY:
                self.minY = line[2]
            if line[2] > self.maxY:
                self.maxY = line[2]
                
    def definirRatio(self):
        largeur = self.maxX-self.minX
        self.ratio = self.largeurImage*self.POINTSPARPOUCE/largeur
        
    def rotater90(self):
        self.lines = [[line[0], line[2], -line[1]] for line in self.lines]
    
    def mettreAZero(self):
        self.definirMinMax()
        self.lines = [[line[0], line[1]-self.minX, line[2]-self.minY] for line in self.lines]

    def mettreAEchelle(self):
        self.lines = [[line[0], line[1]*self.ratio, line[2]*self.ratio] for line in self.lines]

    def decaler(self):
        self.lines = [[line[0], line[1]+self.decalageX*self.POINTSPARPOUCE, line[2]+self.decalageY*self.POINTSPARPOUCE] for line in self.lines]

    def previewTurtle(self):
        largeurMachine = 47.6
        hauteurMachine = 32.2
        winW = 1000
        winH = winW*hauteurMachine/40
        largeur = self.maxX-self.minX
        hauteur = self.maxY-self.minY
        ratio = winW/largeurMachine/self.POINTSPARPOUCE
        tempLines = [[line[0], line[2]*ratio, line[1]*ratio] for line in self.lines]
        turtle.setup(winW+40, winH+40, 0, 0)
        pen = turtle.Turtle()
        pen.up()
        pen.speed(0)
        pen.goto(-winW/2, -winH/2)
        pen.down()
        pen.goto(-winW/2, winH/2)
        pen.goto(winW/2,winH/2)
        pen.goto(winW/2,-winH/2)
        pen.goto(-winW/2, -winH/2)
        pen.up()
        pen.speed(3)
        pen.down()
        horsLimite = False
        for line in tempLines:
            if abs(line[1]) < winW and abs(line[2]) < winH:
                if horsLimite:
                    pen.up()
                    pen.goto(-(line[1]-winW/2), line[2]-winH/2)
                    horsLimite= False
                if line[0] == "PU":
                    pen.up()
                    pen.goto(-(line[1]-winW/2), line[2]-winH/2)
                    pen.down()
                    pen.goto(-(line[1]-winW/2), line[2]-winH/2)
                else:
                    pen.down()
                    pen.goto(-(line[1]-winW/2), line[2]-winH/2)
            else:
                pen.up()
                horsLimite = True
                print(line[1])
        pen.up()
        pen.speed(1)
        pen.goto(-(winW/2-20), winH/2-20)
        turtle.done()
    def convertirEnString(self):
        self.lines = [line[0] + str(int(line[1])) +',' + str(int(line[2])) +";\n" for line in self.lines]
        
    def tracer(self):
        chaine = lireFichier(self.fichierFin)
        chaine += ''.join(self.lines)
        chaine += lireFichier(self.fichierFin)
        ecrireFichier(self.fichierSortie, chaine)
        out1 = "mode " + self.numeroPort + " 19200, n, 8, 1, p"
        out2 = 'copy /b ' + self.fichierSortie + ' ' +self.numeroPort
        #os.system(out1)
        #os.system(out2)
        print(out1)
        print(out2)
        os.remove(self.fichierSortie)
    
    def genererLines(self):
        self.genererTuples()
        self.definirMinMax()
        self.definirRatio()
        self.rotater90()
        self.mettreAZero()
        self.mettreAEchelle()
        self.decaler()
        
    def procedureApercu(self):
        self.genererLines()
        self.previewTurtle()

    def procedureTracer(self):
        self.genererLines()
        self.convertirEnString()
        self.tracer()

form_class = uic.loadUiType("untitled.ui")[0]                 # Load the UI
 
class MyWindowClass(QtGui.QMainWindow, form_class):
    def __init__(self, parent=None):
        QtGui.QMainWindow.__init__(self, parent)
        self.setupUi(self)
        self.btnParcourir.clicked.connect(self.btn_Parcourir_clicked)  # Bind the event handlers
        self.btnApercu.clicked.connect(self.btn_Apercu_clicked)
        self.btnImprimer.clicked.connect(self.btn_Imprimer_clicked)
        self.btnQuitter.clicked.connect(self.btn_Quitter_clicked)

    def bonnesValeurs(self, decalageX, decalageY, largeurImage, numeroPort, nomFichier):
        if not decalageX.isnumeric():
            return False
        elif not decalageY.isnumeric():
            return False
        elif not largeurImage.isnumeric():
            return False
        elif numeroPort.upper() not in ["COM"+str(i) for i in range(1, 5)]:
            return False
        elif not nomFichier.endswith(".plt"):
            return False
        return True

    def btn_Parcourir_clicked(self):
        fname = QtGui.QFileDialog.getOpenFileName(self, 'Open file')
        self.labelFichier.setText(fname)
        
    def getTrace(self):
        if self.bonnesValeurs(self.entryEspaceX.text(), \
                              self.entryEspaceY.text(), \
                              self.entryLargeur.text(), \
                              self.entryPort.text(), \
                              self.labelFichier.text()):
            nomFichier = self.labelFichier.text()
            decalageX = float(self.entryEspaceX.text())
            decalageY = float(self.entryEspaceY.text())
            largeurImage = float(self.entryLargeur.text())
            numeroPort = self.entryPort.text()
            return Trace(decalageX, decalageY, largeurImage, numeroPort, nomFichier)
        return None
    
    def btn_Apercu_clicked(self):
        trace = self.getTrace()
        if trace != None:
            trace.procedureApercu()
        
    def btn_Imprimer_clicked(self):
        trace = self.getTrace()
        if trace != None:
            trace.procedureTracer()
    def btn_Quitter_clicked(self):
        self.close()
    
app = QtGui.QApplication(sys.argv)
myWindow = MyWindowClass(None)
myWindow.show()
app.exec_()
