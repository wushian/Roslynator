@echo off

rem dotnet install tool -g orang.dotnet.cli

set _apiVersion=1.0.0.21
set _version=2.3.0.0
set _version3=2.3.0
set _root=..\src
set _options=from-file -t m r -y t -v n -o "orang.log" v=di

orang replace ^
  "%_root%\Analyzers\Analyzers.csproj" ^
  "%_root%\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj" ^
  "%_root%\CodeFixes\CodeFixes.csproj" ^
  "%_root%\Common\Common.csproj" ^
  "%_root%\Workspaces.Common\Workspaces.Common.csproj" ^
  "%_root%\Refactorings\Refactorings.csproj" ^
 -c "patterns\csproj_version.txt" ^
  %_options% ^
 -r %_version% 

echo.

set _options=%_options% append

orang replace ^
  "%_root%\Core\Core.csproj" ^
  "%_root%\CSharp\CSharp.csproj" ^
  "%_root%\CSharp.Workspaces\CSharp.Workspaces.csproj" ^
  "%_root%\VisualBasic\VisualBasic.csproj" ^
  "%_root%\VisualBasic.Workspaces\VisualBasic.Workspaces.csproj" ^
  "%_root%\Workspaces.Core\Workspaces.Core.csproj" ^
 -c "patterns\csproj_version.txt" ^
  %_options% ^
 -r %_apiVersion%

echo.

orang replace ^
  "%_root%\VisualStudio\source.extension.vsixmanifest" ^
  "%_root%\VisualStudio.Refactorings\source.extension.vsixmanifest" ^
 -c "patterns\vsix_manifest_version.txt" ^
  %_options% ^
 -r %_version3%

echo.

orang replace ^
  "%_root%\VisualStudio\Properties\AssemblyInfo.cs" ^
  "%_root%\VisualStudio.Common\Properties\AssemblyInfo.cs" ^
  "%_root%\VisualStudio.Refactorings\Properties\AssemblyInfo.cs" ^
 -c "patterns\assembly_info_version.txt" ^
  %_options% ^
 -r %_version%

echo.

orang replace ^
  "%_root%\VisualStudioCode\package\package.json" ^
 -c "patterns\vscode_version.txt" ^
  %_options% ^
 -r %_version3%

echo.

orang replace ^
  build.cmd ^
 -c "patterns\build_script_version.txt" ^
  %_options% ^
 -r %_version%

pause
