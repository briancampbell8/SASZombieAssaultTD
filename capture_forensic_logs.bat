@echo off
cd /d "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

echo Starting forensic logging capture...
echo.

REM Clear any existing log file
if exist forensic_log.md del forensic_log.md

echo Running application and monitoring log file growth...
echo.

REM Start the application in background
start /B SASZombieAssaultTD.exe

REM Monitor the log file for 15 seconds
set /a counter=0
:monitor_loop
if exist forensic_log.md (
    for %%A in (forensic_log.md) do (
        echo [%%counter%%s] File size: %%~zA bytes
        if %%~zA GTR 200 (
            echo ✅ Significant logging detected!
            goto show_results
        )
    )
) else (
    echo [%%counter%%s] File not yet created...
)

timeout /t 1 /nobreak > nul
set /a counter+=1
if %counter% LSS 15 goto monitor_loop

echo.
echo Killing application...
taskkill /F /IM SASZombieAssaultTD.exe > nul 2>&1

:show_results
echo.
echo ========================================
echo FORENSIC LOG ANALYSIS:
echo ========================================

if exist forensic_log.md (
    for %%A in (forensic_log.md) do echo Final file size: %%~zA bytes
    
    if %%~zA GTR 0 (
        echo ✅ Log data captured!
        echo.
        echo Log contents:
        echo ====================
        type forensic_log.md
        echo ====================
    ) else (
        echo ❌ Log file is empty
    )
) else (
    echo ❌ No log file created
)

echo.
echo Capture completed.
pause
