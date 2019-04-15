::@echo off
xcopy /E /Y /I Resizer\bin\Debug\netcoreapp2.1 Resizer\bin\Debug\1
cd Resizer\bin\Debug\1
dotnet Resizer.dll
pause