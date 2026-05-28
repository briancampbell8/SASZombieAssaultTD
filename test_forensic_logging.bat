@echo off
echo Starting SAS Zombie Assault TD with forensic logging capture...
echo.

cd /d "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

echo Running application and capturing output to forensic_log.txt...
.\SASZombieAssaultTD.exe > forensic_log.txt 2>&1

echo.
echo Application finished. Check forensic_log.txt for captured output.
echo.

type forensic_log.txt

pause
