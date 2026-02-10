$root = "E:\BDC\Projects\SASZombieAssaultTD"

$expected = @(
    "$root\OriginalSWF\SWF_Backup",
    "$root\OriginalSWF\SWF_Working",

    "$root\Assets\Battlefields",

    "$root\Assets\Zombies\Walker\Walk",
    "$root\Assets\Zombies\Walker\Attack",
    "$root\Assets\Zombies\Walker\Death",

    "$root\Assets\Zombies\Runner\Walk",
    "$root\Assets\Zombies\Runner\Attack",
    "$root\Assets\Zombies\Runner\Death",

    "$root\Assets\Zombies\Tank\Walk",
    "$root\Assets\Zombies\Tank\Attack",
    "$root\Assets\Zombies\Tank\Death",

    "$root\Assets\Zombies\Special\Walk",
    "$root\Assets\Zombies\Special\Attack",
    "$root\Assets\Zombies\Special\Death",

    "$root\Assets\Towers\Assault",
    "$root\Assets\Towers\Sniper",
    "$root\Assets\Towers\Flamethrower",
    "$root\Assets\Towers\Rocket",
    "$root\Assets\Towers\Support",

    "$root\Assets\Effects\Blood",
    "$root\Assets\Effects\Explosions",
    "$root\Assets\Effects\MuzzleFlashes",
    "$root\Assets\Effects\HitEffects",

    "$root\Assets\Bullets\Assault",
    "$root\Assets\Bullets\Sniper",
    "$root\Assets\Bullets\Rocket",
    "$root\Assets\Bullets\Flamethrower",

    "$root\Assets\UI\HUD",
    "$root\Assets\UI\Buttons",
    "$root\Assets\UI\Icons",
    "$root\Assets\UI\AbilitySymbols",

    "$root\Docs\Decompiled",
    "$root\Docs\FieldNotes.md"
)

Write-Host "=== EXTRACTION VALIDATION REPORT ==="

$missing = $expected | Where-Object { -not (Test-Path $_) }
if ($missing.Count -eq 0) {
    Write-Host "Missing expected items: None"
} else {
    Write-Host "Missing expected items:"
    $missing | ForEach-Object { Write-Host "  $_" }
}

$allExtraction = Get-ChildItem -Path "$root\Assets","$root\Docs","$root\OriginalSWF" -Recurse -Directory | Select-Object -ExpandProperty FullName
$unexpected = $allExtraction | Where-Object { $_ -notin $expected }
if ($unexpected.Count -eq 0) {
    Write-Host "Unexpected folders: None"
} else {
    Write-Host "Unexpected folders:"
    $unexpected | ForEach-Object { Write-Host "  $_" }
}

$empty = $allExtraction | Where-Object { (Get-ChildItem $_ -Force | Measure-Object).Count -eq 0 }
if ($empty.Count -eq 0) {
    Write-Host "Empty folders: None"
} else {
    Write-Host "Empty folders:"
    $empty | ForEach-Object { Write-Host "  $_" }
}

Write-Host "=== EXTRACTION VALIDATION COMPLETE ==="