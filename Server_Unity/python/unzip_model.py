import zipfile
import os
current = os.getcwd()
p = os.path.join(current,'pretrained_models','location_predictor_trained')
filename = os.path.join(p ,'transformer1000_10ep.7z')
#f = zipfile.ZipFile(filename,'r') 
#for file in f.namelist():
#    f.extract(file,p)              
#f.close()


import py7zr

archive = py7zr.SevenZipFile(filename, mode='r')
archive.extractall(path=p)
archive.close()