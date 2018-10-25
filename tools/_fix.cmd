@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

set _analyzersDir=..\src\Analyzers.CodeFixes\bin\Debug\netstandard1.3\

"..\src\CommandLine\bin\Debug\net461\roslynator" fix "..\src\CommandLine.sln" ^
 --use-roslynator-analyzers ^
 --ignore-analyzer-references ^
 --format ^
 --log-file "roslynator.log" ^
 --log-file-verbosity d ^
 --diagnostic-fix-map "RCS1155=Roslynator.RCS1155.OrdinalIgnoreCase

pause
