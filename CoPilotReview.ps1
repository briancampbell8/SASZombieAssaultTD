# =====================================================================
# Program: Get_EngineFolderTree.ps1
# Author: Copilot (BDC Workflow Partner)
# Purpose:
#   Generates a deterministic folder/file tree for the following paths:
#       Engine/Rendering
#       Engine/UI
#       Engine/UI/Rendering
#       Engine/UI/Components
#       Engine/UI/Modern
#
#   Output is written to:
#       Engine_FolderTree.txt
#
# Doctrine:
#   - No partial output
#   - No ambiguity
#   - Deterministic, audit‑friendly formatting
# =====================================================================

# --- CONFIG -----------------------------------------------------------
$RootPath = "E:\BDC\Projects\SASZombieAssaultTD\Engine"
$OutputFile = "Engine_FolderTree.txt"

$TargetFolders = @(
    "Rendering",
    "UI",
    "UI\Rendering",
    "UI\Components",
    "UI\Modern"
)

# --- FUNCTION: Write folder tree -------------------------------------
function Write-FolderTree {
    param(
        [string]$Path,
        [int]$Indent = 0
    )

    $prefix = " " * $Indent
    $folderName = Split-Path $Path -Leaf
    Add-Content -Path $OutputFile -Value ("$prefix[$folderName]")

    # Files
    Get-ChildItem -Path $Path -File | ForEach-Object {
        Add-Content -Path $OutputFile -Value ("$prefix  - " + $_.Name)
    }

    # Subfolders
    Get-ChildItem -Path $Path -Directory | ForEach-Object {
        Write-FolderTree -Path $_.FullName -Indent ($Indent + 2)
    }
}

# --- EXECUTION --------------------------------------------------------
if (Test-Path $OutputFile) {
    Remove-Item $OutputFile -Force
}

Add-Content -Path $OutputFile -Value "=== ENGINE FOLDER TREE ==="
Add-Content -Path $OutputFile -Value "Generated: $(Get-Date)"
Add-Content -Path $OutputFile -Value ""

foreach ($folder in $TargetFolders) {
    $full = Join-Path $RootPath $folder
    if (Test-Path $full) {
        Add-Content -Path $OutputFile -Value ">>> $folder"
        Write-FolderTree -Path $full -Indent 2
        Add-Content -Path $OutputFile -Value ""
    }
    else {
        Add-Content -Path $OutputFile -Value ">>> $folder (NOT FOUND)"
        Add-Content -Path $OutputFile -Value ""
    }
}

Add-Content -Path $OutputFile -Value "=== END OF TREE ==="

Write-Host "Folder tree written to $OutputFile"
