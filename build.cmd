@echo off
chcp 65001 >nul
cd /d "%~dp0"
where dotnet >nul 2>&1
if errorlevel 1 (
  echo .NET SDK 10 не найден. Сначала установи SDK: https://dotnet.microsoft.com/download/dotnet/10.0
  pause
  exit /b 1
)
echo === Проверка преобразований ===
dotnet run --project Tests\Tests.csproj -c Release
if errorlevel 1 goto failed
echo === Сборка автономной версии Windows x64 ===
dotnet publish TextSwap\TextSwap.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o publish
if errorlevel 1 goto failed
echo.
echo Готово: publish\TextSwap.exe
pause
exit /b 0
:failed
echo Ошибка проверки или сборки! Смотри сообщение выше.
pause
exit /b 1
