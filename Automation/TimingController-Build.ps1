# Automation/TimingController-Build.ps1
# Structure-first, additive-only, UTF-8 (no BOM)

$ErrorActionPreference = 'Stop'

$root = Get-Location
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

# Paths
$csPath        = Join-Path $root 'Engine/Core/TimingController.cs'
$logPath       = Join-Path $root 'PowerShellLog.md'

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Append-Line {
    param([string]$Path, [string]$Line)
    Add-Content -Path $Path -Value $Line -Encoding UTF8
}

# ---------------------------------------------------------
# 1. Create TimingController.cs if missing
# ---------------------------------------------------------

$initialContent = @"
using System;

namespace Engine.Core
{
    /// <summary>
    /// TimingController
    /// Manages delta time, fixed updates, and frame pacing.
    /// Structure-first stub.
    /// </summary>
    public class TimingController
    {
        public float DeltaTime { get; private set; }

        public void Update(float delta)
        {
            DeltaTime = delta;
        }
    }
}
"@

$created = $false

if (-not (Test-Path $csPath)) {
    [System.IO.File]::WriteAllText($csPath, $initialContent, $utf8NoBom)
    $created = $true
}

# ---------------------------------------------------------
# 2. Log execution
# ---------------------------------------------------------

Append-Line $logPath "
Append-Line $logPath "[$timestamp] TimingController-Build.ps1 executed."
if ($created) {
    Append-Line $logPath "- Created TimingController.cs"
} else {
    Append-Line $logPath "- TimingController.cs already existed (skipped)"
}
Append-Line $logPath "- Structure-first, additive-only."
