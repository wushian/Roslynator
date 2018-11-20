@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

set _analyzersDir=..\src\Analyzers.CodeFixes\bin\Debug\netstandard1.3\

"..\src\CommandLine\bin\Debug\net461\roslynator" analyze-unused "..\src\CommandLine.sln" ^
 --visibility internal ^
 --scope type ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause
