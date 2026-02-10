# =====================================================================
# TreeToMarkdown.ps1
# Generates a Markdown file containing the output of `tree /F`
# =====================================================================

param(
    [string]$TargetPath = ".",
    [string]$OutputFile = "ProjectStructure.md"
)

# Run tree and capture output
$treeOutput = tree $TargetPath /F

# Convert to Markdown fenced code block
$md = @"
# Project Structure

\`\`\`
$treeOutput
\`\`\`
"@

# Write to file (ASCII-safe)
Set-Content -Path $OutputFile -Value $md -Encoding ASCII

Write-Host "Markdown structure written to $OutputFile"
