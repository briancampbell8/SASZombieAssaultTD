@echo off
echo ========================================
echo SAS ZOMBIE ASSAULT TD - FINALIZE BUILD
echo ========================================
echo.

echo This will finalize today.png (case-insensitive) with timestamp
echo and preserve it as the final build of the day.
echo.

REM ----------------------------------------
REM Locate today.png in ANY casing
REM ----------------------------------------
set "source=today.png"

if not exist "%source%" (
    echo ERROR: Could not find today.png
    echo Make sure the game has created the working snapshot.
    dir /b
    pause
    exit /b 1
)

echo Found working file: %source%
echo.

REM ----------------------------------------
REM Get current date/time in stable format
REM ----------------------------------------
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YYYY=%dt:~0,4%"
set "MM=%dt:~4,2%"
set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%"
set "Min=%dt:~10,2%"
set "Sec=%dt:~12,2%"

set "timestamp=%YYYY%-%MM%-%DD%_%HH%-%Min%-%Sec%"
set "target=today_%timestamp%.png"

echo Current time: %timestamp%
echo Target file: %target%
echo.

REM ----------------------------------------
REM Check if target already exists
REM ----------------------------------------
if exist "%target%" (
    echo WARNING: %target% already exists!
    echo It will be overwritten.
    echo.
    set /p "continue=Continue? (y/n): "
    if /I not "%continue%"=="y" (
        echo Operation cancelled.
        pause
        exit /b 1
    )
)

REM ----------------------------------------
REM Perform the rename
REM ----------------------------------------
echo Renaming "%source%" to "%target%"...
ren "%source%" "%target%"

if errorlevel 1 (
    echo ERROR: Failed to rename file!
    pause
    exit /b 1
)

echo.
echo SUCCESS! Build finalized as: %target%
echo.

echo File size:
for %%A in ("%target%") do echo %%~zA bytes
echo.

echo ========================================
echo FINAL BUILD COMPLETE
echo ========================================
echo.

echo Finalized snapshots in this directory:
echo.
dir /b today_*.png
echo.

pause