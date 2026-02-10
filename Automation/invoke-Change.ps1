<#
.NAME
    Invoke-Change.ps1

.PURPOSE
    Execute the highest existing Change####.ps1 script in the Changes directory,
    followed by its Verify and Fix scripts (if present).
#>

param(
    [string]$ChangesPath = ".\Automation\Changes"
)

# Directory MUST exist
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
    throw "No Change####.ps1 files found — nothing to invoke."
}

# Highest existing Change number
$highest = ($numbers | Measure-Object -Maximum).Maximum
$highestStr = $highest.ToString("0000")

# Build filenames
$changeFile = Join-Path $ChangesPath "Change$highestStr.ps1"
$verifyFile = Join-Path $ChangesPath "Verify-Change$highestStr.ps1"
$fixFile = Join-Path $ChangesPath "Change$highestStr-Fix.ps1"

Write-Host "Invoking Change$highestStr..." -ForegroundColor Cyan
& $changeFile

# Run Verify if it exists
if (Test-Path $verifyFile) {
    Write-Host "Running Verify-Change$highestStr..." -ForegroundColor Yellow
    & $verifyFile
}
else {
    Write-Host "Verify-Change$highestStr.ps1 not found — skipping verification." -ForegroundColor DarkYellow
}

# Run Fix if it exists
if (Test-Path $fixFile) {
    Write-Host "Running Change$highestStr-Fix..." -ForegroundColor Magenta
    & $fixFile
}
else {
    Write-Host "Change$highestStr-Fix.ps1 not found — no fix script to run." -ForegroundColor DarkMagenta
}

Write-Host "Completed Change$highestStr execution." -ForegroundColor Green
& "$PSScriptRoot\Generate-ProjectStructure.ps1"