::@echo off
xcopy /E /Y /I Resizer\bin\Debug\netcoreapp2.1 Resizer\bin\Debug\2
cd Resizer\bin\Debug\2
dotnet Resizer.dll
pause