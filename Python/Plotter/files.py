def lireFichier(nomFichier):
    lecture = open(nomFichier, 'r')
    res = lecture.read()
    lecture.close()
    return res

def ecrireFichier(nomFichier, chaine):
    ecriture = open(nomFichier, 'w')
    ecriture.write(chaine)
    ecriture.close()
