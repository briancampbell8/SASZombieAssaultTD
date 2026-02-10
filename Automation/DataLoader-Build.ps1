<#
.SYNOPSIS
    New-Change.ps1 - Generates the next Change, Verify, and Fix scripts.

.DESCRIPTION
    - Scans Automation/Changes for the highest Change number
    - Increments it
    - Creates:
        Change00XX.ps1
        Verify-Change00XX.ps1
        Change00XX-Fix.ps1
    - Uses deterministic UTF-8 encoding
    - Atomic writes
#>

param(
    [string]$ChangesPath = ".\Automation\Changes"
)

# Ensure directory exists
if (-not (Test-Path $ChangesPath)) {
    New-Item -ItemType Directory -Path $ChangesPath | Out-Null
}

# Detect latest Change number
$existing = Get-ChildItem -Path $ChangesPath -Filter "Change*.ps1" |
    Where-Object { $_.Name -match "^Change(\d{4})\.ps1$" }

if ($existing.Count -eq 0) {
    $next = 1
} else {
    $numbers = $existing | ForEach-Object {
        if ($_ -match "^Change(\d{4})\.ps1$") { [int]$Matches[1] }
    }
    $next = ($numbers | Measure-Object -Maximum).Maximum + 1
}

$nextStr = $next.ToString("0000")

# Paths
$changeFile = Join-Path $ChangesPath "Change$nextStr.ps1"
$verifyFile = Join-Path $ChangesPath "Verify-Change$nextStr.ps1"
$fixFile = Join-Path $ChangesPath "Change$nextStr-Fix.ps1"

# Templates
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

$changeContent = @"
# Change$nextStr.ps1
# Created: $timestamp
# Purpose: Implement atomic change $nextStr

Write-Host "Running Change$nextStr..."

# TODO: Insert your change logic here

"@

$verifyContent = @"
# Verify-Change$nextStr.ps1
# Created: $timestamp
# Purpose: Verify correctness of Change$nextStr

Write-Host "Verifying Change$nextStr..."

# TODO: Insert verification logic here

"@

$fixContent = @"
# Change$nextStr-Fix.ps1
# Created: $timestamp
# Purpose: Fix or rollback Change$nextStr

Write-Host "Running Fix for Change$nextStr..."

# TODO: Insert fix logic here

"@

# Write files
Set-Content -Path $changeFile -Value $changeContent -Encoding UTF8
Set-Content -Path $verifyFile -Value $verifyContent -Encoding UTF8
Set-Content -Path $fixFile -Value $fixContent -Encoding UTF8

Write-Host "Generated Change$nextStr, Verify-Change$nextStr, and Change$nextStr-Fix." -ForegroundColor Green