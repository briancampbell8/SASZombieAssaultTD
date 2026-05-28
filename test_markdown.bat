@echo off
cd /d "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

echo Starting SAS Zombie Assault TD for markdown logging test...
echo.

REM Clear any existing log file
if exist forensic_log.md del forensic_log.md

REM Run the application for a few seconds
start /B SASZombieAssaultTD.exe

REM Wait for 5 seconds
timeout /t 5 /nobreak > nul

REM Kill the application
taskkill /F /IM SASZombieAssaultTD.exe > nul 2>&1

echo.
echo Checking forensic_log.md...
echo.

if exist forensic_log.md (
    echo ✅ forensic_log.md was created!
    for %%A in (forensic_log.md) do echo File size: %%~zA bytes
    
    if %%~zA GTR 0 (
        echo ✅ Log file contains data!
        echo.
        echo Log contents:
        echo ====================
        type forensic_log.md
        echo ====================
    ) else (
        echo ❌ Log file is empty
    )
) else (
    echo ❌ forensic_log.md was not created
)

echo.
echo Test completed.
pause
