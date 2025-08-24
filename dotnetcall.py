# requires pythonnet

import pythonnet
pythonnet.load("coreclr")

import clr
clr.AddReference(r'C:\Code\GithubRepos\InputController\bin\Release\net9.0\InputController.dll')
from MyInputLibrary import InputHelper2
InputHelper2.TypeText("Test_Text is being typed. Amazing!")