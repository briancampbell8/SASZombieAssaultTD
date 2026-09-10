# ===================================================================================================
# FILE: UI-Tree.ps1
# AUTHOR: BDC
# DESC:
#     Generates a full directory tree listing for the UI folder only
#     and writes it to UI-Tree.md with fenced code blocks.
# ===================================================================================================

$root = Get-Location
$uiPath = Join-Path $root "UI"
$output = Join-Path $root "UI-Tree.md"

if (-Not (Test-Path $uiPath)) {
    Write-Host "UI folder not found at: $uiPath"
    exit
}

# Capture tree output for UI folder only
$tree = tree $uiPath /f

# Write to markdown file
"```text" | Out-File $output -Encoding utf8
$tree | Out-File $output -Append -Encoding utf8
"```" | Out-File $output -Append -Encoding utf8

Write-Host "UI tree written to $output"
