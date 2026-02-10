<#
    Fix-EmptyFiles.ps1
    Populates empty or comment-only .cs files with minimal valid class stubs.

    Actions:
      - Detects empty or comment-only files
      - Determines correct namespace from folder structure
      - Generates a minimal class stub
      - Preserves using statements
      - Logs all changes to Automation/EmptyFileFixLog.md

    Scope:
      <ProjectRoot>\Engine\
#>

# Resolve project root (script is inside Automation\)
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

$EngineRoot  = Join-Path $ProjectRoot "Engine"
$LogFile     = Join-Path $ProjectRoot "Automation\EmptyFileFixLog.md"

# Canonical namespace root
$NamespaceRoot = "SASZombieAssaultTD.Engine"

# Collect all .cs files
$CsFiles = Get-ChildItem -Path $EngineRoot -Recurse -Filter "*.cs"

# Log header
Add-Content $LogFile "`n## Empty File Fix Run — $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"

foreach ($file in $CsFiles) {

    $lines = Get-Content $file.FullName

    # Remove blank lines and comment-only lines
    $nonComment = $lines | Where-Object {
        $_.Trim() -ne "" -and -not $_.Trim().StartsWith("//")
    }

    # If file contains real code, skip it
    if ($nonComment.Count -gt 0) {
        continue
    }

    # Determine expected namespace based on folder structure
    $relativePath = $file.FullName.Replace($EngineRoot, "").TrimStart("\")
    $folderParts = Split-Path $relativePath -Parent -Resolve:$false
    $folderParts = $folderParts -replace "\\", "."
    $expectedNamespace = if ($folderParts -eq "") {
        $NamespaceRoot
    } else {
        "$NamespaceRoot.$folderParts"
    }

    # Determine class name from filename
    $className = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)

    # Extract using statements (if any)
    $usingLines = $lines | Where-Object { $_ -match "^using\s+" }

    # Log the fix
    Add-Content $LogFile "Populating empty file: $($file.FullName)"
    Add-Content $LogFile " → Namespace: $expectedNamespace"
    Add-Content $LogFile " → Class: $className"
    Add-Content $LogFile ""

    # Build minimal class stub
    $newContent = @()
    $newContent += $usingLines
    $newContent += ""
    $newContent += "namespace $expectedNamespace"
    $newContent += "{"
    $newContent += "    public class $className"
    $newContent += "    {"
    $newContent += "    }"
    $newContent += "}"

    # Write updated file
    $newContent | Set-Content -Path $file.FullName -Encoding UTF8
}

Write-Host "Empty File Fix Complete" -ForegroundColor Cyan
Write-Host "Log written to Automation\EmptyFileFixLog.md"