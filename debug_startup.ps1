# Debug SAS Zombie Assault TD startup
Write-Host "=== DEBUGGING SAS ZOMBIE ASSAULT TD STARTUP ===" -ForegroundColor Green

Set-Location "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

# Clear logs
if (Test-Path "forensic_log.md") { Remove-Item "forensic_log.md" }
if (Test-Path "debug_console.txt") { Remove-Item "debug_console.txt" }

Write-Host "Starting application with console capture..." -ForegroundColor Cyan

# Start process with output capture
$process = Start-Process -FilePath ".\SASZombieAssaultTD.exe" -RedirectStandardOutput "debug_console.txt" -RedirectStandardError "debug_error.txt" -PassThru

# Monitor both console and forensic logs
for ($i = 1; $i -le 15; $i++) {
    Start-Sleep -Seconds 1
    
    # Check console output
    if (Test-Path "debug_console.txt") {
        $consoleSize = (Get-Item "debug_console.txt").Length
        Write-Host "[$i/15s] Console output: $consoleSize bytes" -ForegroundColor Yellow
    }
    
    # Check error output
    if (Test-Path "debug_error.txt") {
        $errorSize = (Get-Item "debug_error.txt").Length
        Write-Host "[$i/15s] Error output: $errorSize bytes" -ForegroundColor Red
    }
    
    # Check forensic log
    if (Test-Path "forensic_log.md") {
        $forensicSize = (Get-Item "forensic_log.md").Length
        Write-Host "[$i/15s] Forensic log: $forensicSize bytes" -ForegroundColor Green
    }
    
    # If process has exited, break
    if ($process.HasExited) {
        Write-Host "Process exited with code: $($process.ExitCode)" -ForegroundColor Red
        break
    }
}

# Kill if still running
if (!$process.HasExited) {
    Stop-Process -Id $process.Id -Force
    Write-Host "Process terminated" -ForegroundColor Yellow
}

Write-Host "`n=== ANALYSIS RESULTS ===" -ForegroundColor Green

# Show console output
if (Test-Path "debug_console.txt") {
    $consoleSize = (Get-Item "debug_console.txt").Length
    Write-Host "📄 CONSOLE OUTPUT ($consoleSize bytes):" -ForegroundColor Cyan
    Write-Host "=============================" -ForegroundColor Gray
    Get-Content "debug_console.txt"
    Write-Host "=============================" -ForegroundColor Gray
} else {
    Write-Host "❌ No console output captured" -ForegroundColor Red
}

# Show error output
if (Test-Path "debug_error.txt") {
    $errorSize = (Get-Item "debug_error.txt").Length
    Write-Host "📄 ERROR OUTPUT ($errorSize bytes):" -ForegroundColor Red
    Write-Host "=============================" -ForegroundColor Gray
    Get-Content "debug_error.txt"
    Write-Host "=============================" -ForegroundColor Gray
} else {
    Write-Host "❌ No error output captured" -ForegroundColor Yellow
}

# Show forensic log
if (Test-Path "forensic_log.md") {
    $forensicSize = (Get-Item "forensic_log.md").Length
    Write-Host "`n📄 FORENSIC LOG ($forensicSize bytes):" -ForegroundColor Cyan
    Write-Host "=========================" -ForegroundColor Gray
    Get-Content "forensic_log.md"
    Write-Host "=========================" -ForegroundColor Gray
} else {
    Write-Host "❌ No forensic log created" -ForegroundColor Red
}

Write-Host "`n🎯 DEBUG COMPLETE!" -ForegroundColor Green
