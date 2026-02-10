<#
    TreeToMarkdown.ps1
    Generates a full directory tree (tree /F) and writes it to Tree-F.md
    in the project root. ASCII-safe, deterministic, and non-destructive.
#>

$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$OutputFile  = Join-Path $ProjectRoot "Tree-F.md"

# Run tree /F and capture output
$treeOutput = tree $ProjectRoot /F

# Write header
"## Project Directory Tree — $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Out-File -FilePath $OutputFile -Encoding ASCII

# Open code fence
'```' | Out-File -FilePath $OutputFile -Append -Encoding ASCII

# Write tree output
$treeOutput | Out-File -FilePath $OutputFile -Append -Encoding ASCII

# Close code fence
'```' | Out-File -FilePath $OutputFile -Append -Encoding ASCII

Write-Host "Tree-F.md generated successfully." -ForegroundColor Cyan