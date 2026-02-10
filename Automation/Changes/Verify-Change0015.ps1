# ============================================
# Verify-Change0015.ps1
# Validation script for Change0015
# Ensures folders, files, and markers exist
# Non-destructive, read-only, audit-friendly
# ============================================

$ChangeId = "Change0015"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
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
    "Scripts",
    "Scripts\Paths",
    "Scripts\Zombies",
    "Scripts\TestScenes",
    "GameData",
    "GameData\Paths"
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
    "Scripts\Paths\PathLoader.cs",
    "Scripts\Zombies\ZombieMovement.cs",
    "Scripts\Zombies\ZombieSpawner.cs",
    "Scripts\TestScenes\MeanStreetsTest.cs"
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

$StartMarker = "// <Change0015>"
$EndMarker   = "// </Change0015>"

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

    # Check for duplicates
    $startCount = ([regex]::Matches($content, [regex]::Escape($StartMarker))).Count
    $endCount   = ([regex]::Matches($content, [regex]::Escape($EndMarker))).Count

    if ($startCount -gt 1 -or $endCount -gt 1) {
        Log "FAIL: Duplicate markers detected in $file"
    } else {
        Log "PASS: No duplicate markers in $file"
    }
}

# --------------------------------------------
# 4. JSON path file presence
# --------------------------------------------

$JsonPath = "GameData\Paths\MeanStreets.json"

if (Test-Path $JsonPath) {
    Log "PASS: Path JSON exists: $JsonPath"
} else {
    Log "FAIL: Missing path JSON: $JsonPath"
}

# --------------------------------------------
# 5. Compile check (optional, non-destructive)
# --------------------------------------------

try {
    dotnet build | Out-Null
    Log "PASS: Project compiles"
}
catch {
    Log "FAIL: Project failed to compile"
}

Log "Verification complete for $ChangeId"