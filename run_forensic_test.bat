@echo off
cd /d "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

echo Starting extended forensic logging test...
echo.

REM Clear any existing log file
if exist forensic_log.md del forensic_log.md

echo Running SAS Zombie Assault TD for 10 seconds to capture full forensic data...
echo.

REM Run the application for 10 seconds
start /B SASZombieAssaultTD.exe

REM Wait for 10 seconds to capture multiple frames
timeout /t 10 /nobreak > nul

REM Kill the application
taskkill /F /IM SASZombieAssaultTD.exe > nul 2>&1

echo.
echo Application stopped. Analyzing forensic logs...
echo.

if exist forensic_log.md (
    echo ✅ forensic_log.md was created!
    for %%A in (forensic_log.md) do echo File size: %%~zA bytes
    
    if %%~zA GTR 0 (
        echo ✅ Log file contains data!
        echo.
        echo ========================================
        echo FORENSIC LOG CONTENTS:
        echo ========================================
        type forensic_log.md
        echo ========================================
        echo.
        echo Log analysis completed.
    ) else (
        echo ❌ Log file is empty
    )
) else (
    echo ❌ forensic_log.md was not created
)

echo.
echo Test completed. Check forensic_log.md for complete forensic data.
pause
