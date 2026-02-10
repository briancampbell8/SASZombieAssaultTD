# ============================================
# Verify-Change0017.ps1
# Validation script for Change0017
# Ensures Animation subsystem + integrations exist
# Non-destructive, read-only, audit-friendly
# ============================================

$ChangeId = "Change0017"
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
    "Engine/Systems/Gameplay/Animation",
    "Scripts/Zombies",
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
    "Engine/Systems/Gameplay/Animation/AnimationFrame.cs",
    "Engine/Systems/Gameplay/Animation/AnimationClip.cs",
    "Engine/Systems/Gameplay/Animation/AnimationPlayer.cs",
    "Scripts/Zombies/ZombieMovement.cs",
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

$StartMarker = "// <Change0017>"
$EndMarker   = "// </Change0017>"

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
# 4. Animation subsystem class validation
# --------------------------------------------

function Check-Class {
    param($file, $class)

    if (!(Test-Path $file)) { return }

    $content = Get-Content $file -Raw
    if ($content -match "class\s+$class") {
        Log "PASS: Class $class found in $file"
    } else {
        Log "FAIL: Class $class missing in $file"
    }
}

Check-Class "Engine/Systems/Gameplay/Animation/AnimationFrame.cs" "AnimationFrame"
Check-Class "Engine/Systems/Gameplay/Animation/AnimationClip.cs" "AnimationClip"
Check-Class "Engine/Systems/Gameplay/Animation/AnimationPlayer.cs" "AnimationPlayer"

# --------------------------------------------
# 5. ZombieMovement integration check
# --------------------------------------------

$ZombieMovementFile = "Scripts/Zombies/ZombieMovement.cs"

if (Test-Path $ZombieMovementFile) {
    $content = Get-Content $ZombieMovementFile -Raw

    if ($content -match "AnimPlayer") {
        Log "PASS: ZombieMovement contains AnimPlayer field"
    } else {
        Log "FAIL: ZombieMovement missing AnimPlayer field"
    }

    if ($content -match "AttachAnimation") {
        Log "PASS: ZombieMovement contains AttachAnimation method"
    } else {
        Log "FAIL: ZombieMovement missing AttachAnimation method"
    }
}

# --------------------------------------------
# 6. GameScene animation update hook
# --------------------------------------------

$SceneFile = "Engine/Scenes/GameScene.cs"

if (Test-Path $SceneFile) {
    $content = Get-Content $SceneFile -Raw

    if ($content -match "AnimPlayer") {
        Log "PASS: GameScene updates animation"
    } else {
        Log "FAIL: GameScene missing animation update hook"
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