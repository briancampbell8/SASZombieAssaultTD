$root = "E:\BDC\Projects\SASZombieAssaultTD"

$expected = @(
    "$root\Core",
    "$root\Core\Config",
    "$root\Core\Managers",
    "$root\Core\Utilities",

    "$root\Engine",
    "$root\Engine\Core",
    "$root\Engine\Entities",
    "$root\Engine\Managers",
    "$root\Engine\Platform",
    "$root\Engine\Rendering",
    "$root\Engine\Scenes",
    "$root\Engine\Systems",
    "$root\Engine\Tools",
    "$root\Engine\UI",

    "$root\Entities",
    "$root\Entities\Abilities",
    "$root\Entities\AirSupport",
    "$root\Entities\Environment",
    "$root\Entities\Graphics",
    "$root\Entities\Graphics\Animations",
    "$root\Entities\Graphics\Backgrounds",
    "$root\Entities\Graphics\Particles",
    "$root\Entities\Graphics\Sprites",
    "$root\Entities\Graphics\UIArt",
    "$root\Entities\Projectiles",
    "$root\Entities\Soldiers",
    "$root\Entities\Towers",
    "$root\Entities\Zombies",

    "$root\Scenes",
    "$root\Scenes\Game",
    "$root\Scenes\MainMenu",
    "$root\Scenes\Pause",
    "$root\Scenes\Shared",

    "$root\Systems",
    "$root\Systems\AI",
    "$root\Systems\Collision",
    "$root\Systems\Combat",
    "$root\Systems\Economy",
    "$root\Systems\Placement",
    "$root\Systems\Survival",
    "$root\Systems\Upgrades",
    "$root\Systems\Waves",

    "$root\Tools",
    "$root\Tools\Harry",
    "$root\Tools\Harry\Changes",

    "$root\UI",
    "$root\UI\Components",
    "$root\UI\HUD",
    "$root\UI\Layouts",
    "$root\UI\Menus"
)

$ignore = @(
    "$root\.vs",
    "$root\bin",
    "$root\obj",
    "$root\Logs",
    "$root\LogAnalysisReports",
    "$root\Automation",
    "$root\Automation\Changes",
    "$root\Automation\Docs",
    "$root\Automation\Logs",
    "$root\Automation\Modules",
    "$root\Automation\Templates"
)

Write-Host "=== PROJECT VALIDATION REPORT ==="

$all = Get-ChildItem -Path $root -Recurse -Directory | Select-Object -ExpandProperty FullName
$filtered = $all | Where-Object { $_ -notlike "$root\bin*" -and $_ -notlike "$root\obj*" -and $_ -notlike "$root\.vs*" }

$missing = $expected | Where-Object { -not (Test-Path $_) }
if ($missing.Count -eq 0) {
    Write-Host "Missing expected items: None"
} else {
    Write-Host "Missing expected items:"
    $missing | ForEach-Object { Write-Host "  $_" }
}

$unexpected = $filtered | Where-Object { $_ -notin $expected -and $_ -notin $ignore }
if ($unexpected.Count -eq 0) {
    Write-Host "Unexpected folders: None"
} else {
    Write-Host "Unexpected folders:"
    $unexpected | ForEach-Object { Write-Host "  $_" }
}

$empty = $filtered | Where-Object { (Get-ChildItem $_ -Force | Measure-Object).Count -eq 0 }
if ($empty.Count -eq 0) {
    Write-Host "Empty folders: None"
} else {
    Write-Host "Empty folders:"
    $empty | ForEach-Object { Write-Host "  $_" }
}

Write-Host "=== PROJECT VALIDATION COMPLETE ==="