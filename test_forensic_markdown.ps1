# Test script for forensic markdown logging
Write-Host "Starting forensic markdown logging test..."

# Navigate to executable directory
Set-Location "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

# Clear any existing log file
if (Test-Path "forensic_log.md") {
    Remove-Item "forensic_log.md"
}

Write-Host "Starting SAS Zombie Assault TD..."

# Start the application
$process = Start-Process -FilePath ".\SASZombieAssaultTD.exe" -PassThru

# Wait for 3 seconds to allow logging to occur
Start-Sleep -Seconds 3

# Terminate the process gracefully
if ($process -and !$process.HasExited) {
    Stop-Process -Id $process.Id -Force
}

Write-Host "Application stopped. Checking forensic log..."

# Check if log file was created
if (Test-Path "forensic_log.md") {
    Write-Host "✅ forensic_log.md created successfully!"
    
    # Show file size
    $size = (Get-Item "forensic_log.md").Length
    Write-Host "File size: $size bytes"
    
    if ($size -gt 0) {
        Write-Host "✅ Log file contains data!"
        Write-Host "Log contents:"
        Write-Host "================"
        Get-Content "forensic_log.md"
        Write-Host "================"
    } else {
        Write-Host "❌ Log file is empty"
    }
} else {
    Write-Host "❌ forensic_log.md was not created"
}

Write-Host "Test completed."
