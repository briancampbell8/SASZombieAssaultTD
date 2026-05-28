@echo off
cd /d "%~dp0"

:: Run the engine and write all output (stdout + stderr) to a Markdown file
dotnet run > hud_debug.md 2>&1

echo Done - check hud_debug.md