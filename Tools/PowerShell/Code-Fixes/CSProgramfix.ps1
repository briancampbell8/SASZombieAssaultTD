param(
    [Parameter(Mandatory=$true)]
    [string]$ErrorReportPath,     # Path to BuildErrors.xlsx exported as CSV or XLSX
    [Parameter(Mandatory=$true)]
    [string]$OutputRoot           # Folder OUTSIDE the repo where cleaned files will be written
)

Write-Host "=== P80 CLEANUP: STARTING STRUCTURAL SANITIZATION ==="

# Ensure output folder exists
if (!(Test-Path $OutputRoot)) {
    New-Item -ItemType Directory -Path $OutputRoot | Out-Null
}

# Load error report (supports CSV or XLSX)
$ext = [System.IO.Path]::GetExtension($ErrorReportPath).ToLower()

if ($ext -eq ".csv") {
    $errors = Import-Csv $ErrorReportPath
}
elseif ($ext -eq ".xlsx") {
    # Requires ImportExcel module
    if (-not (Get-Module -ListAvailable -Name ImportExcel)) {
        throw "ImportExcel module required for XLSX input. Install with: Install-Module ImportExcel"
    }
    $errors = Import-Excel $ErrorReportPath
}
else {
    throw "Unsupported file type. Use CSV or XLSX."
}

# Extract unique file paths
$files = $errors.File | Sort-Object -Unique

Write-Host "Found $($files.Count) corrupted files to clean."

# Define patterns to remove
$invalidLinePatterns = @(
    '^\s*[\)\(]\s*$',                # orphan parentheses
    '^\s*[\{\}]\s*$',                # orphan braces
    '^\s*catch\s*$',                 # orphan catch
    '^\s*finally\s*$',               # orphan finally
    '^\s*return\s*;\s*$',            # orphan return
    '^\s*"ERROR"\s*$',               # stray logging fragments
    '^\s*"INFO"\s*$',                # stray logging fragments
    '^\s*if\s*\(?\s*\)?\s*$',        # broken if
    '^\s*else\s*$',                  # orphan else
    '^\s*[,;]\s*$',                  # stray punctuation
    '^\s*\.\s*$',                    # stray dot
    '^\s*<.*>$',                     # docx/xml artifacts
    '^\s*Tuple.*$',                  # broken tuple fragments
    '^\s*\$.*$',                     # broken interpolated fragments
    '^\s*=>\s*$',                    # orphan lambda arrow
    '^\s*using\s*;\s*$'              # broken using
)

foreach ($file in $files) {

    if (!(Test-Path $file)) {
        Write-Host "Skipping missing file: $file"
        continue
    }

    Write-Host "Cleaning: $file"

    $content = Get-Content $file -Raw
    $lines = $content -split "`r?`n"

    $cleaned = New-Object System.Collections.Generic.List[string]

    foreach ($line in $lines) {
        $trim = $line.Trim()

        # Remove zero-width spaces, smart quotes, docx artifacts
        $safe = $line.
            Replace([char]0x201C,'"').
            Replace([char]0x201D,'"').
            Replace([char]0x2018,"'").
            Replace([char]0x2019,"'").
            Replace([string][char]0xFEFF, "").
            Replace([string][char]0x200B, "")


        $remove = $false

        foreach ($pattern in $invalidLinePatterns) {
            if ($safe -match $pattern) {
                $remove = $true
                break
            }
        }

        if (-not $remove) {
            $cleaned.Add($safe)
        }
    }

    # Output cleaned file OUTSIDE the repo
    $relative = Split-Path $file -NoQualifier
    $outputPath = Join-Path $OutputRoot $relative

    $outputDir = Split-Path $outputPath
    if (!(Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }

    $cleaned -join "`r`n" | Set-Content -Path $outputPath -Encoding UTF8

    Write-Host " → Cleaned file written to: $outputPath"
}

Write-Host "=== P80 CLEANUP COMPLETE ==="
Write-Host "Next step: Replace each corrupted file with its cleaned version, then run build again."