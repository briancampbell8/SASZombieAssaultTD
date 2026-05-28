# FIX-01: Replace SASZombieAssaultTD.Engine.Math with System.Math
$files = @(
    "e:\BDC\Projects\SASZombieAssaultTD\Engine\Scenes\Battlefields\BattlefieldScene.cs",
    "e:\BDC\Projects\SASZombieAssaultTD\Engine\Save\SaveDataExtended.cs",
    "e:\BDC\Projects\SASZombieAssaultTD\Engine\Scenes\SceneTransition.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $content = $content -replace 'SASZombieAssaultTD\.Engine\.Math\.Min', 'System.Math.Min'
        $content = $content -replace 'SASZombieAssaultTD\.Engine\.Math\.Max', 'System.Math.Max'
        $content = $content -replace 'SASZombieAssaultTD\.Engine\.Math\.Clamp', 'System.Math.Clamp'
        Set-Content $file -Value $content -NoNewline
        Write-Host "Fixed math namespace in $file"
    }
}
