@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" ^
 /t:Build ^
 /p:Configuration=Debug,TreatWarningsAsErrors=true,WarningsNotAsErrors="1591" ^
 /v:minimal ^
 /m

if errorlevel 1 (
 pause
 exit
)

"..\src\CommandLine\bin\Debug\net461\roslynator" format "..\src\Roslynator.sln"

pause
