@echo off
set VCVAR="C:\Program Files\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"

call %VCVAR% %2

devenv /rebuild "%1|%2" "..\..\Oatc.OpenMI.Gui.2008.sln"
exit
