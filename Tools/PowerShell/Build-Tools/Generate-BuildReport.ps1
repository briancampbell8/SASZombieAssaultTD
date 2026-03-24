# ============================================
# Generate-BuildReport.ps1
# ============================================
# Runs dotnet build, captures all errors,
# parses them, and outputs a clean CSV.
# "E:\BDC\Projects\SASZombieAssaultTD\Project Documentation\CSV Files\BuildErrors.csv"
# ============================================

param(
    [string]$ProjectPath = "E:\BDC\Projects\SASZombieAssaultTD",
    [string]$OutputCsv = ".\Project Documentation\CSV Files\BuildErrors.csv"
)

Write-Host "Running dotnet build against: $ProjectPath"
Write-Host "Output CSV: $OutputCsv"

# Run build and capture stderr
$buildOutput = dotnet build $ProjectPath 2>&1

# Parse compiler errors
$errors = foreach ($line in $buildOutput) {

    # Typical format:
    # E:\path\file.cs(123,45): error CS1002: ; expected
    if ($line -match '^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\):\s+error\s+(?<code>CS\d+):\s+(?<msg>.+)$') {
        [pscustomobject]@{
            File        = $matches['file']
            Line        = $matches['line']
            Column      = $matches['col']
            ErrorCode   = $matches['code']
            Message     = $matches['msg']
        }
    }
}

if ($errors.Count -eq 0) {
    Write-Host "No compiler errors detected."
} else {
    Write-Host "Errors found: $($errors.Count)"
    $errors | Export-Csv -Path $OutputCsv -NoTypeInformation -Encoding UTF8
    Write-Host "CSV written to: $OutputCsv"
}