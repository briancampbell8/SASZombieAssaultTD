<#
    Change0019-Fix.ps1
    Purpose: Rewrite all Change0019 files with clean, deterministic content.
#>

Write-Host "[Change0019-Fix] Starting Change0019 fix process..."

# Detect project root
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
while (-not (Test-Path "$root\SASZombieAssaultTD.csproj")) {
    $root = Split-Path -Parent $root
    if ($root -eq "") {
        Write-Host "[Change0019-Fix] ERROR: Could not locate project root."
        exit 1
    }
}

Write-Host "[Change0019-Fix] Project root detected: $root"

$files = @{
    "Scripts\TestScenes\MeanStreetsTest.cs" = "// Change0019 FIXED placeholder content"
    "Scripts\Zombies\ZombieMovement.cs"     = "// Change0019 FIXED placeholder content"
    "Scripts\Paths\PathLoader.cs"           = "// Change0019 FIXED placeholder content"
    "Scripts\Zombies\ZombieSpawner.cs"      = "// Change0019 FIXED placeholder content"
}

foreach ($file in $files.Keys) {
    $path = Join-Path $root $file

    if (Test-Path $path) {
        Set-Content -Path $path -Value $files[$file] -Encoding ASCII
        Write-Host "[Change0019-Fix] Rewrote file: $file"
    } else {
        Write-Host "[Change0019-Fix] WARNING: File missing, creating: $file"
        New-Item -ItemType File -Path $path -Force | Out-Null
        Set-Content -Path $path -Value $files[$file] -Encoding ASCII
        Write-Host "[Change0019-Fix] Created + wrote file: $file"
    }
}

Write-Host "[Change0019-Fix] Completed Change0019 fix."