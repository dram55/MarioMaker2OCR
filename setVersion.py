import sys
import os

# Update AssemblyInfo.cs before building.
# example > python setVersion.py 1.0.34
if len(sys.argv) == 2:
	version = sys.argv[1]
	
	currentPath = os.path.dirname(os.path.abspath(__file__))	
	assemblyInfo = os.path.normpath('MarioMaker2OCR/Properties/AssemblyInfo.cs')
	assemblyInfoPath = os.path.join(currentPath, assemblyInfo)
	
	inFile = open(assemblyInfoPath, 'r').read()
	outFile = open(assemblyInfoPath, 'w')
	
	inFile = inFile.replace('0.0.0.0', version)
	
	outFile.write(inFile);
	outFile.close();
else:
	print "Unexpected number of args. Expected 2, got ", len(sys.argv)