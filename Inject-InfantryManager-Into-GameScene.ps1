# Inject-InfantryManager-Into-GameScene.ps1
# Adds InfantryManager field, initialization, and update call to GameScene.cs

$path = "Engine\GameScene.cs"

if (-not (Test-Path $path)) {
    Write-Host "ERROR: GameScene.cs not found at $path"
    exit
}

$content = Get-Content $path -Raw

# 1. Insert field
if ($content -notmatch "InfantryManager") {
    $content = $content -replace "(public class GameScene\s*\{)", "`$1`r`n        private InfantryManager _infantryManager;"
    Write-Host "Added InfantryManager field."
}

# 2. Insert initialization
if ($content -notmatch "_infantryManager = new InfantryManager") {
    $content = $content -replace "(public GameScene\s*\([^\)]*\)\s*\{)", "`$1`r`n            _infantryManager = new InfantryManager();"
    Write-Host "Added InfantryManager initialization."
}

# 3. Insert update call
if ($content -notmatch "_infantryManager.Update") {
    $content = $content -replace "(public void Update\s*\([^\)]*\)\s*\{)", "`$1`r`n            _infantryManager.Update(deltaTime);"
    Write-Host "Added InfantryManager update call."
}

# Write updated file
Set-Content -Path $path -Value $content -Encoding UTF8
Write-Host "GameScene.cs updated successfully."

