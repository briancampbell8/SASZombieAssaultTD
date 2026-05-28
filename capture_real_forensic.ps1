# Real forensic logging capture
Write-Host "=== REAL FORENSIC LOGGING CAPTURE ===" -ForegroundColor Green

Set-Location "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

# Clear existing log
if (Test-Path "forensic_log.md") {
    Remove-Item "forensic_log.md"
    Write-Host "Cleared existing log file" -ForegroundColor Yellow
}

Write-Host "Starting SAS Zombie Assault TD..." -ForegroundColor Cyan

# Start the application
$process = Start-Process -FilePath ".\SASZombieAssaultTD.exe" -PassThru

# Monitor for 10 seconds
Write-Host "Monitoring forensic log for 10 seconds..." -ForegroundColor Cyan
for ($i = 1; $i -le 10; $i++) {
    Start-Sleep -Seconds 1
    if (Test-Path "forensic_log.md") {
        $size = (Get-Item "forensic_log.md").Length
        Write-Host "[$i/10s] Log file size: $size bytes" -ForegroundColor Green
    } else {
        Write-Host "[$i/10s] Log file not yet created..." -ForegroundColor Yellow
    }
}

# Terminate the application
if ($process -and !$process.HasExited) {
    Stop-Process -Id $process.Id -Force
    Write-Host "Application terminated" -ForegroundColor Yellow
}

Write-Host "`n=== FORENSIC LOG ANALYSIS ===" -ForegroundColor Green

if (Test-Path "forensic_log.md") {
    $size = (Get-Item "forensic_log.md").Length
    Write-Host "✅ Log file created!" -ForegroundColor Green
    Write-Host "Final file size: $size bytes" -ForegroundColor Green
    
    if ($size -gt 0) {
        Write-Host "`n📄 FORENSIC LOG CONTENTS:" -ForegroundColor Cyan
        Write-Host "========================" -ForegroundColor Gray
        Get-Content "forensic_log.md"
        Write-Host "========================" -ForegroundColor Gray
    } else {
        Write-Host "❌ Log file is empty" -ForegroundColor Red
    }
} else {
    Write-Host "❌ No log file was created" -ForegroundColor Red
}

Write-Host "`n🎯 CAPTURE COMPLETE!" -ForegroundColor Green
