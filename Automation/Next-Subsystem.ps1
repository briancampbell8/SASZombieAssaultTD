param(
    [string]$CompletedTask = "Timing Controller",
    [string]$NextClassName = "GameStateManager",
    [string]$TargetDir = "Engine"
)

# Paths
$projectRoot = Split-Path -Parent $PSScriptRoot
$tasklistPath = Join-Path $projectRoot "Tasklist.md"
$structurePath = Join-Path $projectRoot "ProjectStructure.md"
$readmePath = Join-Path $projectRoot "README.md"
$logPath = Join-Path $projectRoot "PowerShellLog.md"
$enginePath = Join-Path $projectRoot $TargetDir
$filePath = Join-Path $enginePath "$NextClassName.cs"

# -------------------------
# 1. Mark Completed Task as Reviewed
# -------------------------
$lines = Get-Content $tasklistPath -Encoding UTF8
$escaped = [regex]::Escape($CompletedTask)

for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match "^\s*-\s*$escaped\s*:\s*\[ \]") {
        $lines[$i] = "- $CompletedTask: [x]"
        break
    }
}

[System.IO.File]::WriteAllLines($tasklistPath, $lines, (New-Object System.Text.UTF8Encoding($false)))

Add-Content $logPath "$(Get-Date) � Marked '$CompletedTask' as reviewed."
Write-Host "Marked '$CompletedTask' as reviewed." -ForegroundColor Green

# -------------------------
# 2. Create Next Subsystem File
# -------------------------
if (-not (Test-Path $enginePath)) {
    New-Item -ItemType Directory -Path $enginePath | Out-Null
}

if (-not (Test-Path $filePath)) {

$code = @"
using System;

namespace SASZombieAssaultTD.Engine
{
    public class $NextClassName
    {
        public enum GameState
        {
            None,
            Loading,
            MainMenu,
            Playing,
            Paused,
            GameOver
        }

        private GameState _current;

        public GameState Current => _current;

        public void SetState(GameState state)
        {
            _current = state;
        }

        public bool Is(GameState state)
        {
            return _current == state;
        }
    }
}
"@

    [System.IO.File]::WriteAllText($filePath, $code, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "$NextClassName.cs created." -ForegroundColor Green
}
else {
    Write-Host "$NextClassName.cs already exists. No overwrite performed." -ForegroundColor Yellow
}

# -------------------------
# 3. Update ProjectStructure.md
# -------------------------
$structureLines = Get-Content $structurePath -Encoding UTF8

if ($structureLines -notcontains "    - $NextClassName.cs") {

    $engineIndex = -1
    for ($i = 0; $i -lt $structureLines.Count; $i++) {
        if ($structureLines[$i] -match "Engine/") {
            $engineIndex = $i
            break
        }
    }

    if ($engineIndex -ge 0) {
        $before = $structureLines[0..$engineIndex]
        $after  = $structureLines[($engineIndex+1)..($structureLines.Count-1)]
        $structureLines = $before + "    - $NextClassName.cs" + $after

        [System.IO.File]::WriteAllLines($structurePath, $structureLines, (New-Object System.Text.UTF8Encoding($false)))
        Write-Host "ProjectStructure.md updated." -ForegroundColor Green
    }
}

# -------------------------
# 4. Update README.md
# -------------------------
$readmeLines = Get-Content $readmePath -Encoding UTF8

$engineHeaderIndex = -1
for ($i = 0; $i -lt $readmeLines.Count; $i++) {
    if ($readmeLines[$i] -match "Engine Architecture Overview") {
        $engineHeaderIndex = $i
        break
    }
}

if ($engineHeaderIndex -ge 0 -and $readmeLines -notcontains "- Game State Manager") {
    $before = $readmeLines[0..$engineHeaderIndex]
    $after  = $readmeLines[($engineHeaderIndex+1)..($readmeLines.Count-1)]
    $readmeLines = $before + "- Game State Manager" + $after

    [System.IO.File]::WriteAllLines($readmePath, $readmeLines, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "README.md updated." -ForegroundColor Green
}

# -------------------------
# 5. Log the Action
# -------------------------
Add-Content $logPath "$(Get-Date) � Created $NextClassName.cs and updated ProjectStructure.md and README.md."
Write-Host "Log updated." -ForegroundColor Green
