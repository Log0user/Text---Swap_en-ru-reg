@echo off
setlocal
cd /d "%~dp0"

rem Keep this script ASCII-only for compatibility with cmd.exe on all code pages.
where dotnet >nul 2>&1
if errorlevel 1 goto no_sdk

echo === Running keyboard conversion tests ===
dotnet run --project "Tests\Tests.csproj" --configuration Release
if errorlevel 1 goto failed

echo === Publishing self-contained Windows x64 executable ===
dotnet publish "TextSwap\TextSwap.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o "publish"
if errorlevel 1 goto failed

echo.
echo Success! The executable is at publish\TextSwap.exe
pause
exit /b 0

:no_sdk
echo .NET SDK not found. Install the .NET 10 SDK x64 from:
echo https://dotnet.microsoft.com/download/dotnet/10.0
echo Close and reopen this terminal after installing the SDK.
pause
exit /b 1

:failed
echo Build or tests failed. See the errors above.
pause
exit /b 1
