# ============================================
# Verify-Change0016.ps1
# Validation script for Change0016
# Ensures Path Debug Overlay subsystem is correct
# Non-destructive, read-only, audit-friendly
# ============================================

$ChangeId = "Change0016"
$LogFile = "PowerShellLog.md"

function Log {
    param([string]$Message)
    Add-Content -Path $LogFile -Value "[$ChangeId-Verify] $Message"
}

Log "Starting verification for $ChangeId"

# --------------------------------------------
# 1. Expected folder structure
# --------------------------------------------

$ExpectedFolders = @(
    "Engine",
    "Engine/Rendering",
    "Engine/Rendering/Debug",
    "Engine/Scenes"
)

foreach ($folder in $ExpectedFolders) {
    if (Test-Path $folder) {
        Log "PASS: Folder exists: $folder"
    } else {
        Log "FAIL: Missing folder: $folder"
    }
}

# --------------------------------------------
# 2. Expected files
# --------------------------------------------

$ExpectedFiles = @(
    "Engine/Rendering/Debug/PathDebugRenderer.cs",
    "Engine/Rendering/DebugOverlay.cs",
    "Engine/Scenes/GameScene.cs"
)

foreach ($file in $ExpectedFiles) {
    if (Test-Path $file) {
        Log "PASS: File exists: $file"
    } else {
        Log "FAIL: Missing file: $file"
    }
}

# --------------------------------------------
# 3. Marker validation
# --------------------------------------------

$StartMarker = "// <Change0016>"
$EndMarker   = "// </Change0016>"

foreach ($file in $ExpectedFiles) {

    if (!(Test-Path $file)) { continue }

    $content = Get-Content $file -Raw

    $hasStart = $content.Contains($StartMarker)
    $hasEnd   = $content.Contains($EndMarker)

    if ($hasStart -and $hasEnd) {
        Log "PASS: Markers present in $file"
    } else {
        Log "FAIL: Missing markers in $file"
    }

    # Duplicate detection
    $startCount = ([regex]::Matches($content, [regex]::Escape($StartMarker))).Count
    $endCount   = ([regex]::Matches($content, [regex]::Escape($EndMarker))).Count

    if ($startCount -gt 1 -or $endCount -gt 1) {
        Log "FAIL: Duplicate markers detected in $file"
    } else {
        Log "PASS: No duplicate markers in $file"
    }
}

# --------------------------------------------
# 4. PathDebugRenderer class validation
# --------------------------------------------

$RendererFile = "Engine/Rendering/Debug/PathDebugRenderer.cs"

if (Test-Path $RendererFile) {
    $content = Get-Content $RendererFile -Raw
    if ($content -match "class PathDebugRenderer") {
        Log "PASS: PathDebugRenderer class found"
    } else {
        Log "FAIL: PathDebugRenderer class missing"
    }
}

# --------------------------------------------
# 5. DebugOverlay integration check
# --------------------------------------------

$OverlayFile = "Engine/Rendering/DebugOverlay.cs"

if (Test-Path $OverlayFile) {
    $content = Get-Content $OverlayFile -Raw
    if ($content -match "PathDebugRenderer") {
        Log "PASS: DebugOverlay contains PathDebugRenderer reference"
    } else {
        Log "FAIL: DebugOverlay missing PathDebugRenderer reference"
    }
}

# --------------------------------------------
# 6. GameScene integration check
# --------------------------------------------

$SceneFile = "Engine/Scenes/GameScene.cs"

if (Test-Path $SceneFile) {
    $content = Get-Content $SceneFile -Raw
    if ($content -match "DrawPath") {
        Log "PASS: GameScene contains DrawPath call"
    } else {
        Log "FAIL: GameScene missing DrawPath call"
    }
}

# --------------------------------------------
# 7. Compile check (non-destructive)
# --------------------------------------------

try {
    dotnet build | Out-Null
    Log "PASS: Project compiles"
}
catch {
    Log "FAIL: Project failed to compile"
}

Log "Verification complete for $ChangeId"