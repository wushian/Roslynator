@echo off

set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net461\roslynator" analyze-unused "..\src\Roslynator.sln" ^
 --msbuild-path %_msbuildPath% ^
 --visibility internal ^
 --scope type ^
 --ignore-attributes "System.ObsoleteAttribute" ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause
