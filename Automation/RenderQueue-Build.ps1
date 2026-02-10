param(
    [string]$CompletedTask = "Game State Manager",
    [string]$NextClassName = "RenderQueue",
    [string]$TargetDir = "Engine"
)
CD\
# Paths
$projectRoot = Split-Path -Parent (Split-Path -Parent $PSCommandPath)
$taskListPath = Join-Path $projectRoot "Tasklist.md"
$structurePath = Join-Path $projectRoot "ProjectStructure.md"
$readmePath = Join-Path $projectRoot "README.md"
$logPath = Join-Path $projectRoot "PowerShellLog.md"
$enginePath = Join-Path $projectRoot $TargetDir
$filePath = Join-Path $enginePath "$NextClassName.cs"
Write-Host "$'projectRoot =' $projectRoot"
Write-Host "$'taskListPath =' $taskListPath"
# 1. Mark Completed Task as Reviewed
$lines = Get-Content $taskListPath -Encoding UTF8
$escaped = [regex]::Escape($CompletedTask)

for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match "^\s*-\s*$escaped\s*:\s*\[ \]") {
        $lines[$i] = "- ${CompletedTask}: [x]"

        break
    }
}
[System.IO.File]::WriteAllLines($taskListPath, $lines, (New-Object System.Text.UTF8Encoding($false)))
Add-Content $logPath "$(Get-Date) � Marked '$CompletedTask' as reviewed."
Write-Host "Marked '$CompletedTask' as reviewed." -ForegroundColor Green

# 2. Create RenderQueue.cs
if (-not (Test-Path $enginePath)) {
    New-Item -ItemType Directory -Path $enginePath | Out-Null
}

if (-not (Test-Path $filePath)) {
$code = @"
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine
{
    public class $NextClassName
    {
        private readonly Queue<Action> _drawQueue = new Queue<Action>();

        public void Enqueue(Action drawCall)
        {
            if (drawCall != null)
                _drawQueue.Enqueue(drawCall);
        }

        public void Execute()
        {
            while (_drawQueue.Count > 0)
            {
                var drawCall = _drawQueue.Dequeue();
                drawCall.Invoke();
            }
        }

        public void Clear()
        {
            _drawQueue.Clear();
        }
    }
}
"@
    [System.IO.File]::WriteAllText($filePath, $code, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "$NextClassName.cs created." -ForegroundColor Green
} else {
    Write-Host "$NextClassName.cs already exists. No overwrite performed." -ForegroundColor Yellow
}

# 3. Update ProjectStructure.md
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

# 4. Update README.md
$readmeLines = Get-Content $readmePath -Encoding UTF8
$engineHeaderIndex = -1
for ($i = 0; $i -lt $readmeLines.Count; $i++) {
    if ($readmeLines[$i] -match "Engine Architecture Overview") {
        $engineHeaderIndex = $i
        break
    }
}
if ($engineHeaderIndex -ge 0 -and $readmeLines -notcontains "- Render Queue") {
    $before = $readmeLines[0..$engineHeaderIndex]
    $after  = $readmeLines[($engineHeaderIndex+1)..($readmeLines.Count-1)]
    $readmeLines = $before + "- Render Queue" + $after
    [System.IO.File]::WriteAllLines($readmePath, $readmeLines, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "README.md updated." -ForegroundColor Green
}

# 5. Log the Action
Add-Content $logPath "$(Get-Date) � Created $NextClassName.cs and updated ProjectStructure.md and README.md."
Write-Host "Log updated." -ForegroundColor Green
