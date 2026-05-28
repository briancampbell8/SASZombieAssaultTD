# Focused forensic capture of PresentFramebuffer() sequence
Write-Host "=== PRESENT FRAMEBUFFER FORENSIC CAPTURE ===" -ForegroundColor Green

Set-Location "E:\BDC\Projects\SASZombieAssaultTD\bin\Debug\net8.0-windows"

# Clear existing logs
if (Test-Path "forensic_log.md") { Remove-Item "forensic_log.md" }
if (Test-Path "console_output.txt") { Remove-Item "console_output.txt" }
if (Test-Path "console_error.txt") { Remove-Item "console_error.txt" }

Write-Host "Starting application for PresentFramebuffer analysis..." -ForegroundColor Cyan

# Start process with full console capture
$process = Start-Process -FilePath ".\SASZombieAssaultTD.exe" -RedirectStandardOutput "console_output.txt" -RedirectStandardError "console_error.txt" -PassThru

# Monitor for PresentFramebuffer completion
Write-Host "Monitoring for PresentFramebuffer completion..." -ForegroundColor Cyan
$foundPresent = $false

for ($i = 1; $i -le 8; $i++) {
    Start-Sleep -Seconds 1
    
    if (Test-Path "console_output.txt") {
        $content = Get-Content "console_output.txt" -Raw
        $size = (Get-Item "console_output.txt").Length
        Write-Host "[$i/8s] Console output: $size bytes" -ForegroundColor Yellow
        
        # Check for PresentFramebuffer completion
        if ($content -match "DXGI Present return") {
            Write-Host "✅ Found DXGI Present return code!" -ForegroundColor Green
            $foundPresent = $true
            break
        }
        
        # Check for PresentFramebuffer END
        if ($content -match "PresentFramebuffer\(\) END") {
            Write-Host "✅ Found PresentFramebuffer END!" -ForegroundColor Green
            $foundPresent = $true
            break
        }
    }
    
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

Write-Host "`n=== PRESENT FRAMEBUFFER ANALYSIS ===" -ForegroundColor Green

if (Test-Path "console_output.txt") {
    $content = Get-Content "console_output.txt" -Raw
    $size = (Get-Item "console_output.txt").Length
    Write-Host "📄 CONSOLE OUTPUT ($size bytes):" -ForegroundColor Cyan
    Write-Host "=============================" -ForegroundColor Gray
    
    # Extract PresentFramebuffer sequence
    $presentLines = $content -split "`n" | Where-Object { $_ -match "PresentFramebuffer|DXGI Present|Present\(\) END|Present\(\) SUCCESS|Present\(\) FAILED" }
    
    if ($presentLines.Count -gt 0) {
        Write-Host "🎯 PRESENT FRAMEBUFFER SEQUENCE:" -ForegroundColor Green
        Write-Host "=============================" -ForegroundColor Gray
        $presentLines | ForEach-Object { Write-Host $_ -ForegroundColor White }
    } else {
        Write-Host "❌ No PresentFramebuffer sequence found" -ForegroundColor Red
        Write-Host "📄 LAST 20 LINES:" -ForegroundColor Yellow
        $content -split "`n" | Select-Object -Last 20 | ForEach-Object { Write-Host $_ -ForegroundColor White }
    }
    
    Write-Host "=============================" -ForegroundColor Gray
} else {
    Write-Host "❌ No console output captured" -ForegroundColor Red
}

Write-Host "`n🎯 ANALYSIS COMPLETE!" -ForegroundColor Green
