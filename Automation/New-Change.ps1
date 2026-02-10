<#
.NAME
    New-Change.ps1

.PURPOSE
    Determine the next Change####.ps1 by:
    - Scanning the directory for all Change####.ps1 files
    - Extracting the numeric portion
    - Finding the highest existing number
    - Adding 1
    - Rebuilding Change####.ps1 with padded zeros
#>

param(
    [string]$ChangesPath = ".\Automation\Changes"
)

# This directory MUST exist. If it doesn't, the project state is invalid.
if (-not (Test-Path $ChangesPath)) {
    throw "Changes directory missing — project state invalid."
}

# Get all Change####.ps1 files
$files = Get-ChildItem $ChangesPath -Filter "Change*.ps1" | Select-Object -ExpandProperty Name

# Extract numeric portions
$numbers = foreach ($f in $files) {
    if ($f -match '^Change(\d{4})\.ps1$') {
        [int]$matches[1]
    }
}

if ($numbers.Count -eq 0) {
    throw "No Change####.ps1 files found — project state invalid."
}

# Highest existing Change number
$highest = ($numbers | Measure-Object -Maximum).Maximum

# Next Change number
$next = $highest + 1
$nextStr = $next.ToString("0000")

# Paths
$changeFile = Join-Path $ChangesPath "Change$nextStr.ps1"
$verifyFile = Join-Path $ChangesPath "Verify-Change$nextStr.ps1"
$fixFile = Join-Path $ChangesPath "Change$nextStr-Fix.ps1"

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Templates
$changeContent = @"
# Change$nextStr.ps1
# Created: $timestamp
# Purpose: Implement atomic change $nextStr

Write-Host "Running Change$nextStr..."
"@

$verifyContent = @"
# Verify-Change$nextStr.ps1
# Created: $timestamp
# Purpose: Verify correctness of Change$nextStr

Write-Host "Verifying Change$nextStr..."
"@

$fixContent = @"
# Change$nextStr-Fix.ps1
# Created: $timestamp
# Purpose: Fix or rollback Change$nextStr

Write-Host "Running Fix for Change$nextStr..."
"@

# Write files
Set-Content -Path $changeFile -Value $changeContent -Encoding UTF8
Set-Content -Path $verifyFile -Value $verifyContent -Encoding UTF8
Set-Content -Path $fixFile -Value $fixContent -Encoding UTF8

Write-Host "Generated Change$nextStr, Verify-Change$nextStr, and Change$nextStr-Fix." -ForegroundColor Green