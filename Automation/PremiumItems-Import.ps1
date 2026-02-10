# PremiumItems-Import.ps1
# Mode: deterministic, audit-friendly, no drift
# Purpose: Rename folder, import PNGs, create structure, log actions

$ScriptName = Split-Path -Leaf $PSCommandPath
$Timestamp  = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$Actions    = @()

Write-Host "=== $ScriptName started at $Timestamp ==="
Write-Host "Mode: deterministic, ASCII, no drift"

# ---------------------------------------------------------
# 1. SOURCE FOLDER (your current PNG location)
# ---------------------------------------------------------
$OldSource = "E:\BDC\Projects\SASZombieAssaultTD\Assets\Sprites\Premium Items"
$NewSource = "E:\BDC\Projects\SASZombieAssaultTD\Assets\Sprites\PremiumItems"

# Rename folder if needed
if (Test-Path $OldSource) {
    Rename-Item -Path $OldSource -NewName "PremiumItems"
    $Actions += "Renamed folder: 'Premium Items' → 'PremiumItems'"
}

# Confirm new source exists
if (!(Test-Path $NewSource)) {
    Write-Host "ERROR: Source folder not found after rename."
    exit
}

$SourceFolder = $NewSource
$Actions += "Using source folder: $SourceFolder"

# ---------------------------------------------------------
# 2. TARGET FOLDERS
# ---------------------------------------------------------
$FlashExtract = "E:\BDC\Projects\SASZombieAssaultTD\Assets\FlashExtract\PremiumItems"
$Sprites      = "E:\BDC\Projects\SASZombieAssaultTD\Assets\Sprites\PremiumItems"
$AtlasSource  = "E:\BDC\Projects\SASZombieAssaultTD\Assets\AtlasSource\PremiumItems"

foreach ($folder in @($FlashExtract, $Sprites, $AtlasSource)) {
    if (!(Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder | Out-Null
        $Actions += "Created folder: $folder"
    }
}

# ---------------------------------------------------------
# 3. FILE LIST
# ---------------------------------------------------------
$Files = @(
    "cryo.png",
    "healthup.png",
    "incendiary.png",
    "longbow.png",
    "mines.png",
    "necro.png",
    "nuke.png",
    "typhoon.png"
)

# ---------------------------------------------------------
# 4. COPY FILES
# ---------------------------------------------------------
foreach ($file in $Files) {
    $src = Join-Path $SourceFolder $file

    if (Test-Path $src) {

        # Archive originals
        $dst1 = Join-Path $FlashExtract $file
        if (!(Test-Path $dst1)) {
            Copy-Item $src $dst1
            $Actions += "Archived original: $file"
        }

        # Runtime folder
        $dst2 = Join-Path $Sprites $file
        Copy-Item $src $dst2 -Force
        $Actions += "Copied to runtime: $file"

        # Atlas input
        $dst3 = Join-Path $AtlasSource $file
        Copy-Item $src $dst3 -Force
        $Actions += "Copied to atlas input: $file"

    } else {
        $Actions += "Missing source file: $file"
    }
}

# ---------------------------------------------------------
# 5. LOGGING
# ---------------------------------------------------------
Add-Content -Path "PowerShellLog.md" -Value @"
## $ScriptName — $Timestamp
- Premium item import
- Actions:
$($Actions | ForEach-Object { "  - $_" })
- Result: SUCCESS
"@

Add-Content -Path "PowerShellLog.txt" -Value "[$Timestamp] $ScriptName executed — SUCCESS"

Write-Host "=== $ScriptName completed successfully ==="
