cd ..\..\..\tools\MinGW
CALL init.bat
cd ..\..\src\app\ctests
gcc TestFile.c -Isoftfloat\include softfloat\softfloat.a
a.exe