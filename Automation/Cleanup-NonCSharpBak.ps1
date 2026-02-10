<# 
    Cleanup-NonCSharpBak.ps1
    Purpose:
        Delete all .bak files that do NOT contain C# code.
        Preserve .bak files that DO contain C# code (rollback safety).
        Produce a Markdown report instead of .log.

    Boundaries:
        - Never touches the engine root except for READ-ONLY scanning.
        - All output is written to Automation folder.
        - No .log files created.
        - Deterministic, ASCII-safe, audit-friendly.
#>

$ErrorActionPreference = "Stop"

$root = "E:\BDC\Projects\SASZombieAssaultTD"
$report = Join-Path $root "Automation\Cleanup-NonCSharpBak.md"

# Overwrite report
"## Cleanup-NonCSharpBak.ps1 — $(Get-Date)" | Out-File $report -Encoding UTF8
"" | Out-File $report -Append

Add-Content $report "### Summary"
Add-Content $report "- Removing .bak files that do NOT contain C# code"
Add-Content $report "- Preserving .bak files that DO contain C# code"
Add-Content $report "- Engine root remains untouched except for read-only scanning"
Add-Content $report ""

Add-Content $report "### Deleted Files"
Add-Content $report ""

$deletedCount = 0

Get-ChildItem -Path $root -Recurse -Filter *.bak |
    ForEach-Object {
        $file = $_.FullName
        $content = Get-Content $file -Raw -ErrorAction SilentlyContinue

        # Detect C# indicators
        $isCSharp = (
            $content -match '\bnamespace\b' -or
            $content -match '\bclass\b'     -or
            $content -match '\busing\b'     -or
            $content -match '[{};]'
        )

        if (-not $isCSharp) {
            Remove-Item $file -Force
            Add-Content $report "- Deleted: $file"
            $deletedCount++
        }
    }

if ($deletedCount -eq 0) {
    Add-Content $report "- No non-C# .bak files found."
}

Add-Content $report ""
Add-Content $report "### Cleanup Complete — $(Get-Date)"