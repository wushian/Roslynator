@echo off

rem "C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"..\src\CommandLine\bin\Debug\net461\roslynator" format "E:\Dokumenty\Visual Studio 2017\Projects\x\dotvvm\src\dotvvm.sln" ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag ^
 --empty-line-after-closing-brace ^
 --empty-line-after-embedded-statement ^
 --empty-line-before-while-in-do-statement ^
 --empty-line-between-declarations ^
 --format-accessor-list ^
 --format-declaration-braces ^
 --format-empty-block ^
 --format-single-line-block ^
 --new-line-after-switch-label ^
 --new-line-before-embedded-statement ^
 --new-line-before-enum-member ^
 --new-line-before-statement ^
 --remove-redundant-empty-line ^
 --end-of-line crlf ^
 --culture en

pause
