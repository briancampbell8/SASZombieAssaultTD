<#
    Fix-Namespaces.ps1
    Corrects namespace drift across the entire Engine directory.

    Actions:
      - Detects missing namespaces
      - Detects incorrect namespaces (Engine.*, no namespace, etc.)
      - Rewrites namespace to SASZombieAssaultTD.Engine.<subfolders>
      - Preserves using statements and code body
      - Logs all changes to Automation/NamespaceFixLog.md

    Scope:
      <ProjectRoot>\Engine\
#>

# Resolve project root (script is inside Automation\)
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

$EngineRoot  = Join-Path $ProjectRoot "Engine"
$LogFile     = Join-Path $ProjectRoot "Automation\NamespaceFixLog.md"

# Canonical namespace root
$NamespaceRoot = "SASZombieAssaultTD.Engine"

# Collect all .cs files
$CsFiles = Get-ChildItem -Path $EngineRoot -Recurse -Filter "*.cs"

# Log header
Add-Content $LogFile "`n## Namespace Fix Run — $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"

foreach ($file in $CsFiles) {

    $lines = Get-Content $file.FullName

    # Extract using statements
    $usingLines = $lines | Where-Object { $_ -match "^using\s+" }

    # Extract the rest (potential namespace + code)
    $nonUsingLines = $lines | Where-Object { $_ -notmatch "^using\s+" }

    # Determine expected namespace based on folder structure
    $relativePath = $file.FullName.Replace($EngineRoot, "").TrimStart("\")
    $folderParts = Split-Path $relativePath -Parent -Resolve:$false
    $folderParts = $folderParts -replace "\\", "."
    $expectedNamespace = if ($folderParts -eq "") {
        $NamespaceRoot
    } else {
        "$NamespaceRoot.$folderParts"
    }

    # Detect existing namespace
    $existingNsLine = $nonUsingLines | Where-Object { $_ -match "^namespace\s+" }

    $needsFix = $false

    if (-not $existingNsLine) {
        $needsFix = $true
    }
    elseif ($existingNsLine -notmatch [regex]::Escape($expectedNamespace)) {
        $needsFix = $true
    }

    if ($needsFix) {

        # Log the change
        Add-Content $LogFile "Fixing: $($file.FullName)"
        Add-Content $LogFile " → Expected: $expectedNamespace"
        Add-Content $LogFile ""

        # Remove old namespace line if present
        $nonUsingLines = $nonUsingLines | Where-Object { $_ -notmatch "^namespace\s+" }

        # Build new file content
        $newContent = @()
        $newContent += $usingLines
        $newContent += ""
        $newContent += "namespace $expectedNamespace"
        $newContent += "{"

        # Indent all code lines by 4 spaces
        foreach ($line in $nonUsingLines) {
            if ($line.Trim() -ne "") {
                $newContent += "    $line"
            }
        }

        $newContent += "}"

        # Write updated file
        $newContent | Set-Content -Path $file.FullName -Encoding UTF8
    }
}

Write-Host "Namespace Fix Complete" -ForegroundColor Cyan
Write-Host "Log written to Automation\NamespaceFixLog.md"